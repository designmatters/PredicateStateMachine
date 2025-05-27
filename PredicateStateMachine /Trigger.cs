namespace PredicateStateMachine;

public class Trigger<TEvent> : ITransition<TEvent> where TEvent : IEvent
{
    public Trigger(Func<TEvent, bool> selector, Func<TEvent, bool>? predicate = null, int priority = 0)
    {
        Selector = selector;
        Predicate = predicate;
        Priority = priority;
    }

    public Func<TEvent, bool> Selector { get; set; }
    public Func<TEvent, bool>? Predicate { get; set; }
    public int Priority { get; }
    public bool CanTransition(TEvent e) => Selector(e) && (Predicate is null || Predicate(e));
}