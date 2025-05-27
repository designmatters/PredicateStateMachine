namespace PredicateStateMachine;

public interface IStateMachineConfig<TEvent> where TEvent : IEvent
{
    IStateNode<TEvent> GetRoot();
}