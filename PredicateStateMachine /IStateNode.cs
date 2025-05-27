namespace PredicateStateMachine;

public interface IStateNode<TEvent> where TEvent : IEvent
{
    string Name { get; set; }
    PredicateStateMachine<TEvent> Owner { get; }
    void AddPath(ITransition<TEvent> transition, IStateNode<TEvent> nextState);
    IStateNode<TEvent> HandleEvent(TEvent e);
    void Start();
}