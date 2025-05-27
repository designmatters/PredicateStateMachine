namespace PredicateStateMachine;

public class StateTimeoutConfiguration<TEvent> where TEvent : IEvent
{
    public StateTimeoutConfiguration(double timeoutMs, TEvent? timeoutEvent)
    {
        TimeoutMs = timeoutMs;
        TimeoutEvent = timeoutEvent;
    }
    public double TimeoutMs { get; set; }
    public TEvent? TimeoutEvent { get; set; }
}