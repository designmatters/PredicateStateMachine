namespace PredicateStateMachine;

public class StateMachineConfig<TEvent> : IStateMachineConfig<TEvent> where TEvent : IEvent
{
    private readonly IStateNode<TEvent> _startStateNode;

    public StateMachineConfig(IStateNode<TEvent> startStateNode)
    {
        _startStateNode = startStateNode;
    }

    public IStateNode<TEvent> GetRoot()
    {
        return _startStateNode;
    }
}