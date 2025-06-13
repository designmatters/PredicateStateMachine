using Microsoft.Extensions.Logging;
using PredicateStateMachine;

namespace Lock;

public static class Example
{
    public static void Run()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Information)
                .AddConsole(options => { options.TimestampFormat = "HH:mm:ss "; });
        });

        var logger = loggerFactory.CreateLogger<PredicateStateMachine<AccessEvent>>();
        
        int failedAttempts = 0;

        var machine = new PredicateStateMachine<AccessEvent>(logger);
        var idle = new AccessState("Idle");
        var checking = new AccessState("Checking");
        var granted = new AccessState("Granted");
        var denied = new AccessState("Denied")
        {
            OnAfterStartAction = () =>
            {
                failedAttempts++;
                if (failedAttempts >= 2)
                {
                    machine.HandleEvent(new AccessEvent("Lockout"));
                    failedAttempts = 0;
                }
            }
        };
        var lockedOut = new AccessState("LockedOut");

        machine.AddPath(idle, new Transition<AccessEvent>(e => e.Identifier == "Code Entered"), checking);
        machine.AddPath(checking, new Transition<AccessEvent>(e => e.Identifier == "Granted"), granted);
        machine.AddPath(checking, new Transition<AccessEvent>(e => e.Identifier == "Denied"), denied);
        machine.AddPath(denied, new Transition<AccessEvent>(e => true), idle);
        machine.AddPath(denied, new Transition<AccessEvent>(e => e.Identifier == "Lockout", priority: 1), lockedOut);
        machine.AddPath(granted, new Transition<AccessEvent>(e => e.Identifier == "Timeout"), idle);
        machine.AddTimeout(granted, new TimeoutConfiguration<AccessEvent>(3000, new AccessEvent("Timeout")));

        machine.AddStates([idle, checking, granted, denied, lockedOut]);
        machine.Configure(new StateMachineConfig<AccessEvent>(idle)); //merge this into prev
        machine.Start();

        machine.HandleEvent(new AccessEvent("Code Entered"));
        machine.HandleEvent(new AccessEvent("Denied"));
        machine.HandleEvent(new AccessEvent("Code Entered"));
        machine.HandleEvent(new AccessEvent("Denied"));
        machine.HandleEvent(new AccessEvent("Code Entered"));
        machine.HandleEvent(new AccessEvent("Denied"));
    }
}