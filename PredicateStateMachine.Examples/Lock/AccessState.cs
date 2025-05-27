using PredicateStateMachine;

namespace Lock;

public class AccessState : StateNode<AccessEvent>
{
    public AccessState(PredicateStateMachine<AccessEvent> stateMachine, string name) : base(stateMachine, name)
    {
        
    }

    protected override void OnStart()
    {
        Console.WriteLine($"→ {Name}");
    }

    protected override void OnStop() { }

    protected override void OnAfterStart() { OnAfterStartAction?.Invoke(); }

    protected override void OnAfterStop() { }

    protected override void OnBeforeStart() { }

    protected override void OnBeforeStop() { }

    public Action? OnAfterStartAction { get; set; }
}