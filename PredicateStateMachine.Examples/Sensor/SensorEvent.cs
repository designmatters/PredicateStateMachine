using PredicateStateMachine;

namespace Sensor;

public class SensorEvent : IEvent
{
    public SensorEvent(string identifier)
    {
        Identifier = identifier;
    }

    public string Identifier { get; set; }
    public double Timeout { get; set; } //DO not use int
}