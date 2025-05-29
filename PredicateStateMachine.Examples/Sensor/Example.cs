using Microsoft.Extensions.Logging;
using PredicateStateMachine;

namespace Sensor;

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

        var logger = loggerFactory.CreateLogger<PredicateStateMachine<SensorEvent>>();

        var machine = new PredicateStateMachine<SensorEvent>(logger);

        var idle = new SensorState("Idle");
        var detected = new SensorState("Detected");
        var alarm = new SensorState("Alarm")
        {
            OnStartAction = () => Console.WriteLine("Alaaaarm!")
        };

        // If the sensor stays in movementDetected for at least 5s go to alarm. 
        machine.AddPath(idle, new Transition<SensorEvent>(e => e is { Identifier: "MovementDetected" }), detected);
        machine.AddPath(detected, new Transition<SensorEvent>(e => e is { Identifier: "MovementCleared" }), idle);
        machine.AddTimeout(detected, new TimeoutConfiguration<SensorEvent>(5000, new SensorEvent("Timeout")));
        machine.AddPath(detected, new Transition<SensorEvent>(e => e is { Identifier: "Timeout" }), alarm);

        
        machine.AddStates([idle, detected, alarm]);
        machine.Configure(new StateMachineConfig<SensorEvent>(idle));
        machine.Start();

        machine.HandleEvent(new SensorEvent("MovementDetected"));
        Thread.Sleep(3000);
        machine.HandleEvent(new SensorEvent("MovementCleared"));
        machine.HandleEvent(new SensorEvent("MovementDetected"));
        Thread.Sleep(6000);
    }
}