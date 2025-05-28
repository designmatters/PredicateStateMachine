using PredicateStateMachine;

namespace ModeratedChat;

public class ModerationState : StateNode<ModerationEvent>
{
    public ModerationState(PredicateStateMachine<ModerationEvent> stateMachine, string name) : base(stateMachine, name)
    {
    }

    protected override void OnStart()
    {
        Console.WriteLine($"→ State: {Name}");
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