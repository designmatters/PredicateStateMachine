namespace PredicateStateMachine;

public interface IStateNode<TEvent, TData>
{
    string Name { get; set; }
    IEnumerable<IStateNode<TEvent, TData>> NextNodes { get; set; }
    TData Data { get; set; }
    int TimeoutMs { get; set; }
    TEvent TimeoutEvent { get; }
    void AddPath(ITransition<TEvent, TData> transition, IStateNode<TEvent, TData> nextState);
    IStateNode<TEvent, TData> HandleEvent(TEvent e);
    void Start(ITransition<TEvent, TData> trigger);
}