using PredicateStateMachine;

namespace EmergencyTrafficLight;

public class TrafficState : IStateNode<TrafficEvent>
{
    public string Name { get; set; }
    public Action? OnAfterStartAction { get; set; }

    public TrafficState(string name)
    {
        Name = name;
    }

    public void OnBeforeStart() { }
    public void OnStart() { Console.WriteLine($"→ {Name}"); }
    public void OnAfterStart() => OnAfterStartAction?.Invoke();
    public void OnBeforeStop() { }
    public void OnStop() { }
    public void OnAfterStop() { }
}
