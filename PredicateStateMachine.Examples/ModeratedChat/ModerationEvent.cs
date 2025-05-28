using PredicateStateMachine;


namespace ModeratedChat;

public class ModerationEvent : IEvent
{
    public ModerationEvent(string type)
    {
        Identifier = type;
    }

    public string Identifier { get; set; }
    public bool Severe { get; set; }
    public double Timeout { get; set; }
}