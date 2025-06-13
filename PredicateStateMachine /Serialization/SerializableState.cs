
namespace PredicateStateMachine.Serialization;

public class SerializableState<TEvent> where TEvent : IEvent
{
    public string Name { get; set; }
}