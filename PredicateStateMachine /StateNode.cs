using System.Timers;
using Timer = System.Timers.Timer;

namespace PredicateStateMachine;

public abstract class StateNode<TEvent> : IStateNode<TEvent> where TEvent : IEvent
{
    private readonly Dictionary<ITransition<TEvent>, IStateNode<TEvent>> _transitions = new();
    private readonly Timer _timeoutTimer;
    private readonly object _lock = new();

    public string Name { get; set; }
    public StateTimeoutConfiguration<TEvent>? TimeoutConfiguration { get; private set; }
    public PredicateStateMachine<TEvent> Owner { get; }

    protected StateNode(PredicateStateMachine<TEvent> stateMachine, string name)
    {
        Owner = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        Name = name ?? throw new ArgumentNullException(nameof(name));

        _timeoutTimer = new Timer { AutoReset = false };
        _timeoutTimer.Elapsed += TimeoutTimerElapsed;
    }
    
    public void AddPath(ITransition<TEvent> transition, IStateNode<TEvent> nextState)
    {
        if (transition == null || nextState == null)
            throw new ArgumentNullException();

        lock (_lock)
        {
            _transitions[transition] = nextState;
        }
    }

    public void AddTimeout(StateTimeoutConfiguration<TEvent> timeoutConfiguration)
    {
        TimeoutConfiguration = timeoutConfiguration ?? throw new ArgumentNullException(nameof(timeoutConfiguration));
    }

    public IStateNode<TEvent> HandleEvent(TEvent e)
    {
        KeyValuePair<ITransition<TEvent>, IStateNode<TEvent>>? selected;

        lock (_lock)
        {
            selected = _transitions
                .Where(t => t.Key.CanTransition(e))
                .OrderByDescending(t => t.Key.Priority)
                .Cast<KeyValuePair<ITransition<TEvent>, IStateNode<TEvent>>?>()
                .FirstOrDefault();
        }

        if (selected.HasValue)
        {
            var next = selected.Value.Value;

            OnBeforeStop();
            StopTimer();
            OnStop();
            OnAfterStop();

            next.Start();
            return next;
        }

        return this;
    }

    public void Start()
    {
        OnBeforeStart();
        Owner.Transition(this);
        StartTimerIfNeeded();
        OnStart();
        OnAfterStart();
    }

    private void StopTimer()
    {
        _timeoutTimer.Stop();
    }

    private void StartTimerIfNeeded()
    {
        if (TimeoutConfiguration != null && TimeoutConfiguration.TimeoutMs > 0.0)
        {
            _timeoutTimer.Interval = TimeoutConfiguration.TimeoutMs;
            _timeoutTimer.Start();
        }
    }

    private void TimeoutTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _timeoutTimer.Stop();

        if (TimeoutConfiguration != null && TimeoutConfiguration.TimeoutEvent != null)
        {
            Owner.HandleEvent(TimeoutConfiguration.TimeoutEvent);
        }
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract void OnAfterStart();
    protected abstract void OnAfterStop();
    protected abstract void OnBeforeStart();
    protected abstract void OnBeforeStop();
}