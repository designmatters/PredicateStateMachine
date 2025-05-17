using System.Timers;
using Timer = System.Timers.Timer;

namespace PredicateStateMachine;

public abstract class StateNode<TEvent, TData> : IStateNode<TEvent, TData>
{
    private readonly Dictionary<ITransition<TEvent, TData>, IStateNode<TEvent, TData>> _transitions = new();

    private readonly Timer _timeoutTimer = new();

    private SimpleStateMachine<TEvent, TData> SimpleStateMachine { get; set; } //name

    protected StateNode(SimpleStateMachine<TEvent, TData> stateMachine)
    {
        SimpleStateMachine = stateMachine;

        _timeoutTimer.Elapsed  += TimeoutTimerElapsed;
    }

    void TimeoutTimerElapsed(object sender, ElapsedEventArgs e)
    {
        _timeoutTimer.Stop();
        SimpleStateMachine.HandleEvent(TimeoutEvent);
    }


    public string Name { get; set; }

    public IEnumerable<IStateNode<TEvent, TData>> NextNodes
    {
        get { return _transitions.Select(t => t.Value); }
        set { throw new NotImplementedException(); } //to check
    }

    public TData Data { get; set; }
    public int TimeoutMs { get; set; }


    public void AddPath(ITransition<TEvent, TData> transition, IStateNode<TEvent, TData> nextState)
    {
        _transitions.Add(transition, nextState);
    }

    public IStateNode<TEvent, TData> HandleEvent(TEvent e)
    {
        var transition = _transitions.Where(t => t.Key.CanTransition(e, Data))
            .OrderByDescending(t => t.Key.Priority).FirstOrDefault();

        if (transition.Key != null)
        {
            IStateNode<TEvent, TData> next = transition.Value;
            OnBeforeStop();
            _StopTimer();
            DoStop();
            next.Start(transition.Key);
            OnAfterStop();
            return next;
        }

        return this;
    }

    public void Start(ITransition<TEvent, TData> trigger)
    {
        OnBeforeStart();

        _StartTimerIfNeeded();
        DoStart(trigger);

        OnAfterStart();
    }


    public void _StopTimer()
    {
        _timeoutTimer.Stop();
    }


    private void _StartTimerIfNeeded()
    {
        if (TimeoutMs != 0)
        {
            _timeoutTimer.Interval = TimeoutMs;
            _timeoutTimer.Start();
        }
    }


    protected abstract void DoStart(ITransition<TEvent, TData> trigger);

    protected abstract void DoStop();
    protected abstract void OnAfterStart();
    protected abstract void OnAfterStop();
    protected abstract void OnBeforeStart();

    protected abstract void OnBeforeStop();


    public abstract TEvent TimeoutEvent { get; }
}