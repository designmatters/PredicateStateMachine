using Microsoft.Extensions.Logging;
using PredicateStateMachine.ActivityMonitor;
using PredicateStateMachine.ActivityMonitor.Impl;
using Timer = System.Timers.Timer;

namespace PredicateStateMachine;

public class PredicateStateMachine<TEvent> : IPredicateStateMachine<TEvent>
    where TEvent : IEvent
{
    private readonly ILogger? _logger;
    private IStateNode<TEvent>? _current;
    private IStateNode<TEvent>? _root;
    private List<IStateNode<TEvent>>? _states;
    private IActivityMonitor<TEvent> _activityMonitor;

    private readonly Dictionary<IStateNode<TEvent>, Dictionary<ITransition<TEvent>, IStateNode<TEvent>>> _paths = new();
    private readonly Dictionary<IStateNode<TEvent>, TimeoutConfiguration<TEvent>> _timeouts = new();
    private readonly Dictionary<IStateNode<TEvent>, Timer> _timers = new();

    private readonly object _lock = new();
    private bool _started;
    // private bool _stopped;

    public PredicateStateMachine(ILogger? logger = null)
    {
        _logger = logger;
        _activityMonitor = new DefaultActivityMonitor<TEvent>(_logger); // for now
    }
    public void AddStates(List<IStateNode<TEvent>> newStates)
    {
        _states ??= [];

        var duplicates = _states.Concat(newStates)
            .GroupBy(s => s.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count > 0)
            throw new ApplicationException($"Every state must have a unique name. Duplicates found: {string.Join(", ", duplicates)}");

        _states.AddRange(newStates);
    }

    public void AddPath(IStateNode<TEvent> sourceState, ITransition<TEvent> transition, IStateNode<TEvent> targetState)
    {
        if (!_paths.TryGetValue(sourceState, out var transitions))
            _paths[sourceState] = transitions = new();

        transitions[transition] = targetState;
    }

    public void AddTimeout(IStateNode<TEvent> sourceState, TimeoutConfiguration<TEvent> timeoutConfiguration)
    {
        _timeouts[sourceState] = timeoutConfiguration;
    }

    public void Configure(IStateMachineConfig<TEvent> config)
    {
        _root = config.GetRoot();
    }

    public IStateNode<TEvent> HandleEvent(TEvent e)
    {
        if (!_started)
            throw new ApplicationException("State machine has not been started.");
        // if (_stopped)
        //     throw new ApplicationException("State machine has been stopped.");

        KeyValuePair<ITransition<TEvent>, IStateNode<TEvent>>? selected = null;

        lock (_lock)
        {
            if (_current == null || !_paths.TryGetValue(_current, out var transitions))
                return _current!;

            selected = transitions
                .Where(t => t.Key.CanTransition(e))
                .OrderByDescending(t => t.Key.Priority)
                .Cast<KeyValuePair<ITransition<TEvent>, IStateNode<TEvent>>?>()
                .FirstOrDefault();
        }

        if (selected.HasValue)
        {
            var nextState = selected.Value.Value;
            _current?.OnBeforeStop();
            StopTimer(_current);
            _current?.OnStop();
            _current?.OnAfterStop();

            nextState.OnBeforeStart();
            _activityMonitor.Register(_current ?? null, e, nextState);
            _current = nextState;
            _current.OnStart();
            StartTimer(_current);
            _current.OnAfterStart();

            return _current;
        }

        return _current!;
    }

    public IStateNode<TEvent> GetCurrentState()
    {
        return _current!;
    }

    public void Start()
    {
        if (_root == null)
            throw new InvalidOperationException("Root state is not set.");
        _activityMonitor.RegisterMachineStart(_current , _root);
        _current = _root;
        _started = true;
        _current.OnStart();
        StartTimer(_current);
    }

    private void StartTimer(IStateNode<TEvent> state)
    {
        if (_timeouts.TryGetValue(state, out var config))
        {
            var timer = new Timer(config.TimeoutMs);
            timer.Elapsed += (_, _) =>
            {
                timer.Stop();
                timer.Dispose();
                _timers.Remove(state);
                lock (_lock)
                {
                    if (_current == state)
                    {
                        HandleEvent(config.TimeoutEvent);
                    }
                }
            };
            timer.AutoReset = false;
            _timers[state] = timer;
            timer.Start();
        }
    }

    private void StopTimer(IStateNode<TEvent> state)
    {
        if (_timers.TryGetValue(state, out var timer))
        {
            timer.Stop();
            timer.Dispose();
            _timers.Remove(state);
        }
    }
}