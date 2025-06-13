namespace PredicateStateMachine.Serialization;

public class SerializableTimeout<TEvent> where TEvent : IEvent
{
    public string StateName { get; set; }
    public double TimeoutMs { get; set; }
    public string TimeoutEvent { get; set; } //create a type
}