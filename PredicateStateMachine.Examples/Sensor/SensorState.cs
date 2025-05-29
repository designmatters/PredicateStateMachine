using PredicateStateMachine;

namespace Sensor;

public class SensorState : IStateNode<SensorEvent>
{
    public string Name { get; set; }
    public Action? OnStartAction { get; set; }

    public SensorState(string name)
    {
        Name = name;
    }

    public void OnBeforeStart() { }
    public void OnStart()
    {
        Console.WriteLine($"→ {Name}");
        OnStartAction?.Invoke();
    }
    public void OnAfterStart() { }
    public void OnBeforeStop() { }
    public void OnStop() { }
    public void OnAfterStop() { }
}