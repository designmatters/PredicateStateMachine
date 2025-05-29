namespace PredicateStateMachine;

public class TimeoutConfiguration<TEvent> where TEvent : IEvent
{
    public TimeoutConfiguration(double timeoutMs, TEvent timeoutEvent)
    {
        TimeoutMs = timeoutMs;
        TimeoutEvent = timeoutEvent;
    }

    public double TimeoutMs { get; set; }
    public TEvent TimeoutEvent { get; set; }
}