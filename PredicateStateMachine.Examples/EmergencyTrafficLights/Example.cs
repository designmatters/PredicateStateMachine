using Microsoft.Extensions.Logging;
using PredicateStateMachine;
using Sensor;

namespace EmergencyTrafficLight;

public static class Example
{
    public static async Task Run()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Information)
                .AddConsole(options => { options.TimestampFormat = "HH:mm:ss "; });
        });

        var logger = loggerFactory.CreateLogger<PredicateStateMachine<TrafficEvent>>();
        
        var machine = new PredicateStateMachine<TrafficEvent>(logger);

        var red = new TrafficState("Red");
        var green = new TrafficState("Green");
        var orange = new TrafficState("Orange");
        var orangeFlashing = new TrafficState("OrangeFlashing");

        machine.AddTimeout(red, new TimeoutConfiguration<TrafficEvent>(5000, new TrafficEvent("Timer")));
        machine.AddPath(red, new Transition<TrafficEvent>(e => e.Identifier == "Timer"), green);

        machine.AddTimeout(green, new TimeoutConfiguration<TrafficEvent>(4000, new TrafficEvent("Timer")));
        machine.AddPath(green, new Transition<TrafficEvent>(e => e.Identifier == "Timer"), orange);

        machine.AddTimeout(orange, new TimeoutConfiguration<TrafficEvent>(2000, new TrafficEvent("Timer")));
        machine.AddPath(orange, new Transition<TrafficEvent>(e => e.Identifier == "Timer"), red);
        
        foreach (var state in new[] { red, green, orange })
        {
            machine.AddPath(state, new Transition<TrafficEvent>(
                e => e.Identifier == "EmergencyDetected" && e.DistanceMeters == 1000, priority: 10), orangeFlashing);
        }

        foreach (var state in new[] { red, green, orange, orangeFlashing })
        {
            machine.AddPath(state, new Transition<TrafficEvent>(
                e => e.Identifier == "EmergencyDetected" && e.DistanceMeters < 500, priority: 20), red);
        }
        
        machine.AddPath(red, new Transition<TrafficEvent>(
            e => e.Identifier == "EmergencyCleared", priority: 5), green);

        machine.AddStates([red, green, orange, orangeFlashing]);
        machine.Configure(new StateMachineConfig<TrafficEvent>(red));
        machine.Start();

        var i = 0;
        while (true)
        {
            await Task.Delay(100);

            if (i++ == 100)
            {
                machine.HandleEvent(new TrafficEvent("EmergencyDetected", 1000));
            }
        }
    }
}
