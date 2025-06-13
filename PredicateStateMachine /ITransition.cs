using System.Linq.Expressions;

namespace PredicateStateMachine;

public interface ITransition<TEvent> where TEvent : IEvent
{
    Expression<Func<TEvent, bool>> Selector { get; set; }
    public Expression<Func<TEvent, bool>>? Predicate { get; }
    int Priority { get; }
    bool CanTransition(TEvent e);
}