namespace PredicateStateMachine.Serialization;

internal interface ISerializableStateMachine<TEvent> where TEvent : IEvent
{
    SerializableStateMachine<TEvent> ToSerializable();
    static abstract PredicateStateMachine<TEvent> FromSerializable(SerializableStateMachine<TEvent> dto);
}