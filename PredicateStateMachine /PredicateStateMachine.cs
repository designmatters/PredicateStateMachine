using Microsoft.Extensions.Logging;
using PredicateStateMachine.ActivityMonitor;
using PredicateStateMachine.ActivityMonitor.Impl;
using ITimer = PredicateStateMachine.Timer.ITimer;

namespace PredicateStateMachine;

public class PredicateStateMachine<TEvent> : IPredicateStateMachine<TEvent>
    where TEvent : IEvent
{
    private readonly ILogger? _logger;
    private IStateNode<TEvent>? _current;
    private IStateNode<TEvent>? _root;
    private IStateNode<TEvent>? _pausedOnState = null;
    private List<IStateNode<TEvent>>? _states;
    private IActivityMonitor<TEvent> _activityMonitor;
    private LifecycleState _lifecycleState = LifecycleState.NotStarted;
    private readonly Dictionary<IStateNode<TEvent>, Dictionary<ITransition<TEvent>, IStateNode<TEvent>>> _paths = new();
    private readonly Dictionary<IStateNode<TEvent>, TimeoutConfiguration<TEvent>> _timeouts = new();
    private readonly Dictionary<IStateNode<TEvent>, ITimer> _timers = new();

    private readonly object _lock = new();
    

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

    public void HandleEvent(TEvent e)
    {
        if (_lifecycleState != LifecycleState.Running && _lifecycleState != LifecycleState.Resumed)
            return;
        
        KeyValuePair<ITransition<TEvent>, IStateNode<TEvent>>? selected = null;

        lock (_lock)
        {
            if (_current == null || !_paths.TryGetValue(_current, out var transitions))
                return;

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

            return;
        }

        return;
    }

    public IStateNode<TEvent> GetCurrentState()
    {
        return _current!;
    }

    public void Start()
    {
        if (_root == null)
            throw new InvalidOperationException("Root state is not set.");
        _lifecycleState = LifecycleState.Running;
        _activityMonitor.RegisterMachineStarted(_current , _root);
        _current = _root;
        _current.OnBeforeStart();
        _current.OnStart();
        _current.OnAfterStart();
        StartTimer(_current);
    }
    
    public void Pause()
    {
        StopTimer(_current); // TODO. What are the specs here. The timer must pause and resume
        _current?.OnBeforeStop();
        _current?.OnStop();
        _pausedOnState = _current;
        _lifecycleState = LifecycleState.Stopped;
        _current?.OnAfterStop();
        _activityMonitor?.RegisterMachinePaused(_current);
        _current = null;
    }
    
    public void Resume()
    {
        _lifecycleState = LifecycleState.Resumed;
        _current = _pausedOnState;
        _activityMonitor.RegisterMachineResumed(_current);
        _current?.OnBeforeStart();
        _current?.OnStart();
        _current?.OnAfterStart();
        if (_current != null)
            StartTimer(_current); // TODO. What are the specs here. The timer must pause and resume
    }

    private void StartTimer(IStateNode<TEvent> state)
    {
        if (_timeouts.TryGetValue(state, out var config))
        {
            var timer = config.GetTimerFactory()();
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