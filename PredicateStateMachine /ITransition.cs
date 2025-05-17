namespace PredicateStateMachine;

public interface ITransition<TEvent, in TData>
{
    TEvent Event { get; }
    Func<TData, bool> Predicate { get; }

    int Priority { get; }

    bool CanTransition(TEvent e, TData payload);
}