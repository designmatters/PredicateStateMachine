using ITimer = PredicateStateMachine.Timer.ITimer;
using PredicateStateMachine.Timer.Impl;

namespace PredicateStateMachine;

public class TimeoutConfiguration<TEvent> where TEvent : IEvent
{
    private readonly Func<ITimer> _timer;

    public TimeoutConfiguration(double timeoutMs, TEvent timeoutEvent, Func<ITimer>? timerFactory = null)
    {
        TimeoutMs = timeoutMs;
        TimeoutEvent = timeoutEvent;
        _timer = timerFactory ?? CreateSystemTimer;
    }

    private ITimer CreateSystemTimer()
    {
        return new SystemTimer(TimeoutMs);
    }

    public double TimeoutMs { get; set; }
    public TEvent TimeoutEvent { get; set; }
    
    internal Func<ITimer> GetTimerFactory ()
    {
        return _timer;
    }
}