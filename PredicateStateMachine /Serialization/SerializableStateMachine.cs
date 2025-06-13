namespace PredicateStateMachine.Serialization;

public class SerializableStateMachine<TEvent> where TEvent : IEvent
{
    public string RootStateName { get; set; }
    public List<SerializableState<TEvent>> States { get; set; } = new();
    public List<SerializableTransition<TEvent>> Transitions { get; set; } = new();
    public List<SerializableTimeout<TEvent>> Timeouts { get; set; } = new();
}