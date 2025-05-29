namespace PredicateStateMachine;

public interface ITransition<TEvent> where TEvent : IEvent
{
    Func<TEvent, bool> Selector { get; set; }
    Func<TEvent, bool> Predicate { get; }
    int Priority { get; }
    bool CanTransition(TEvent e);
}