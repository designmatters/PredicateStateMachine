namespace PredicateStateMachine;

//Drop this if no complex behaviour is needed
public class StateMachineConfig<TEvent, TData> : IStateMachineConfig<TEvent, TData>
{
    private readonly IStateNode<TEvent, TData> _startStateNode;

    public StateMachineConfig(IStateNode<TEvent, TData> startStateNode)
    {
        _startStateNode = startStateNode;
    }
    public IStateNode<TEvent, TData> GetRoot()
    {
        return _startStateNode;
    }
}