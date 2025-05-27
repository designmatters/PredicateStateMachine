using PredicateStateMachine;

namespace Sensor;

public class SensorState : StateNode<SensorEvent>
{
    public SensorState(PredicateStateMachine<SensorEvent> machine, string name)
        : base(machine, name)
    {
        Name = name;
    }

    protected override void OnStart() =>
        Console.WriteLine($"→ {Name}");

    protected override void OnStop()
    {
    }

    protected override void OnBeforeStart() // prev state
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
}