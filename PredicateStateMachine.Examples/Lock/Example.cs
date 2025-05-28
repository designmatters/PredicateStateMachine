using PredicateStateMachine;

namespace Lock;

public static class Example
{
    public static void Run()
    {
        var machine = new PredicateStateMachine<AccessEvent>();

        var idle = new AccessState(machine, "Idle");
        var checking = new AccessState(machine, "Checking");
        var granted = new AccessState(machine, "Granted");
        var denied = new AccessState(machine, "Denied");
        var lockedOut = new AccessState(machine, "LockedOut");

        int failedAttempts = 0;

        idle.AddPath(new Trigger<AccessEvent>(e => e.Identifier == "Code Entered"), checking);
        checking.AddPath(new Trigger<AccessEvent>(
            e => e.Identifier == "Granted"), granted);
        checking.AddPath(new Trigger<AccessEvent>(
            e => e.Identifier == "Denied"), denied);
        denied.AddPath(new Trigger<AccessEvent>(e => true), idle);
        denied.AddPath(new Trigger<AccessEvent>(e => e.Identifier == "Lockout", priority: 1), lockedOut);
        granted.AddTimeout(new StateTimeoutConfiguration<AccessEvent>(3000, new AccessEvent("Timeout")));
        granted.AddPath(new Trigger<AccessEvent>(e => e.Identifier == "Timeout"), idle);

        denied.OnAfterStartAction = () =>
        {
            failedAttempts++;
            if (failedAttempts >= 2)
            {
                machine.HandleEvent(new AccessEvent("Lockout"));
                failedAttempts = 0;
            }
        };


        machine.Configure(new StateMachineConfig<AccessEvent>(idle));
        machine.Start();

        machine.HandleEvent(new AccessEvent("Code Entered"));
        machine.HandleEvent(new AccessEvent("Denied"));
        machine.HandleEvent(new AccessEvent("Code Entered"));
        machine.HandleEvent(new AccessEvent("Denied"));
        machine.HandleEvent(new AccessEvent("Code Entered"));
        machine.HandleEvent(new AccessEvent("Denied"));
    }
}