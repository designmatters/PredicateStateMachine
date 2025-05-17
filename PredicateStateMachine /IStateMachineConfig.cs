namespace PredicateStateMachine;

public interface IStateMachineConfig<TEvent, TData>
{
    IStateNode<TEvent, TData> GetRoot();
}