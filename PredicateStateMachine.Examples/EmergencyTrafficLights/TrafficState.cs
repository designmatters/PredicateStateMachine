using PredicateStateMachine;

namespace EmergencyTrafficLight;

public class TrafficState : StateNode<TrafficEvent>
{
    public TrafficState(PredicateStateMachine<TrafficEvent> stateMachine, string name) : base(stateMachine, name)
    {
        Name = name;
    }

    protected override void OnStart()
    {
        Console.WriteLine($"→ {Name}");
    }

    protected override void OnStop()
    {
    }

    protected override void OnAfterStart()
    {
    }

    protected override void OnAfterStop()
    {
    }

    protected override void OnBeforeStart()
    {
    }

    protected override void OnBeforeStop()
    {
    }
}