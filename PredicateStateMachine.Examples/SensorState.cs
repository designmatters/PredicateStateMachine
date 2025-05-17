using PredicateStateMachine;

public class SensorState : StateNode<SensorEvent, object>
{
    public SensorState(SimpleStateMachine<SensorEvent, object> machine, string name) : base(machine) => Name = name;

    protected override void DoStart(ITransition<SensorEvent, object> trigger) =>
        Console.WriteLine($"→ {Name}");

    protected override void DoStop() =>
        Console.WriteLine($"← {Name}");

    protected override void OnBeforeStart()
    {
    }

    protected override void OnAfterStart()
    {
    }

    protected override void OnBeforeStop()
    {
    }

    protected override void OnAfterStop()
    {
    }

    public override SensorEvent TimeoutEvent => SensorEvent.Timeout;
}