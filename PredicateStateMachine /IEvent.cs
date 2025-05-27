namespace PredicateStateMachine;

public interface IEvent
{
    string Identifier { get; set; }
    public double Timeout { get; set; } 
}