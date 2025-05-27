using PredicateStateMachine;

namespace EmergencyTrafficLight;

public class TrafficEvent : IEvent
{
    public string Identifier { get; set; }
    public double Timeout { get; set; }
    public int DistanceMeters { get; }

    public TrafficEvent(string identifier, int distanceMeters = 0)
    {
        Identifier = identifier;
        DistanceMeters = distanceMeters;
    }
}