using PredicateStateMachine;

namespace ModeratedChat;

public class ModerationState : IStateNode<ModerationEvent>
{
    public string Name { get; set; }

    public ModerationState(string name)
    {
        Name = name;
    }

    public void OnBeforeStart() { }
    public void OnStart() { Console.WriteLine($"→ {Name}"); }
    public void OnAfterStart() { }
    public void OnBeforeStop() { }
    public void OnStop() { }
    public void OnAfterStop() { }
}