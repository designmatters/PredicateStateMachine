using System;
using System.Linq.Expressions;

namespace PredicateStateMachine
{
    public class Transition<TEvent> : ITransition<TEvent> where TEvent : IEvent
    {
        public Transition(Expression<Func<TEvent, bool>> selector, Expression<Func<TEvent, bool>>? predicate = null, int priority = 0)
        {
            Selector = selector;
            Predicate = predicate;
            Priority = priority;
            _compiledSelector = selector.Compile();
            _compiledPredicate = predicate?.Compile();
        }

        public Expression<Func<TEvent, bool>> Selector { get; set; }
        public Expression<Func<TEvent, bool>>? Predicate { get; set; }
        public int Priority { get; }

        private readonly Func<TEvent, bool> _compiledSelector;
        private readonly Func<TEvent, bool>? _compiledPredicate;

        public bool CanTransition(TEvent e)
            => _compiledSelector(e) && (_compiledPredicate == null || _compiledPredicate(e));
    }
}