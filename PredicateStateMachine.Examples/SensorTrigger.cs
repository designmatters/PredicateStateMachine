using PredicateStateMachine;

public class SensorTrigger : ITransition<SensorEvent, object>
{
    public SensorEvent Event { get; }
    public Func<object, bool> Predicate { get; }
    public int Priority { get; }

    public SensorTrigger(SensorEvent e, int priority = 0, Func<object, bool>? predicate = null)
    {
        Event = e;
        Predicate = predicate ?? (_ => true);
        Priority = priority;
    }

    public bool CanTransition(SensorEvent e, object data) => e.Equals(Event) && Predicate(data);
}