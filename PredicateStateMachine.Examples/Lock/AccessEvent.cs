using PredicateStateMachine;

namespace Lock;

// public class AccessEvent : IEvent
// {
//     public AccessEvent(string type)
//     {
//         Identifier = type;
//     }
//
//     public string Identifier { get; set; }
//     public double Timeout { get; set; }
// }

public class AccessEvent : IEvent
{
    public string Identifier { get; set; }
    public double Timeout { get; set; } //needed?

    public AccessEvent(string identifier)
    {
        Identifier = identifier;
    }
}