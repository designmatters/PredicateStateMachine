namespace PredicateStateMachine.Serialization;

public class SerializableTransition<TEvent> where TEvent : IEvent
{
    public string SourceStateName { get; set; }
    public string TargetStateName { get; set; }

    public string Selector { get; set; } //T look at 
    public string? Predicate { get; set; }

    public int Priority { get; set; }
}