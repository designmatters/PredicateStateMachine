using PredicateStateMachine;

namespace Lock;

// public class AccessState : StateNode<AccessEvent>
// {
//     public AccessState(PredicateStateMachine<AccessEvent> stateMachine, string name) : base(stateMachine, name)
//     {
//         
//     }
//
//     protected override void OnStart()
//     {
//         Console.WriteLine($"→ {Name}");
//     }
//
//     protected override void OnStop() { }
//
//     protected override void OnAfterStart() { OnAfterStartAction?.Invoke(); }
//
//     protected override void OnAfterStop() { }
//
//     protected override void OnBeforeStart() { }
//
//     protected override void OnBeforeStop() { }
//
//     public Action? OnAfterStartAction { get; set; }
// }


public class AccessState : IStateNode<AccessEvent>
{
    public string Name { get; set; }
    public Action? OnAfterStartAction { get; set; }

    public AccessState(string name)
    {
        Name = name;
    }

    public void OnBeforeStart() { }

    public void OnStart()
    {
        Console.WriteLine($"→ {Name}");
    }
    public void OnAfterStart() => OnAfterStartAction?.Invoke();
    public void OnBeforeStop() { }
    public void OnStop() { }
    public void OnAfterStop() { }
}