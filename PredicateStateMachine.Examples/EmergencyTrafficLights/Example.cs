using PredicateStateMachine;

namespace EmergencyTrafficLight;

public static class Example
{
    public static async Task Run()
    {
        var machine = new PredicateStateMachine<TrafficEvent>();

        var red = new TrafficState(machine, "Red");
        var green = new TrafficState(machine, "Green");
        var orange = new TrafficState(machine, "Orange");
        var orangeFlashing = new TrafficState(machine, "OrangeFlashing");

        //Normal traffic light
        red.AddTimeout(new StateTimeoutConfiguration<TrafficEvent>(5000, new TrafficEvent("Timer")));
        red.AddPath(new Trigger<TrafficEvent>(e => e.Identifier == "Timer"), green);
        green.AddTimeout(new StateTimeoutConfiguration<TrafficEvent>(4000, new TrafficEvent("Timer")));
        green.AddPath(new Trigger<TrafficEvent>(e => e.Identifier == "Timer"), orange);
        orange.AddTimeout(new StateTimeoutConfiguration<TrafficEvent>(2000, new TrafficEvent("Timer")));
        orange.AddPath(new Trigger<TrafficEvent>(e => e.Identifier == "Timer"), red);

        //Deal with traffic event regarding prioritary vehicles
        //TODO configure to only go to orange flashing when it's green. Otherwise to red and say until EmergencyCleared
        foreach (var state in new[] { red, green, orange })
        {
            state.AddPath(new Trigger<TrafficEvent>(
                    e => e.Identifier == "EmergencyDetected", e => e.DistanceMeters == 1000, priority: 10),
                orangeFlashing);
        }

        foreach (var state in new[] { red, green, orange, orangeFlashing })
        {
            state.AddPath(new Trigger<TrafficEvent>(
                    e => e.Identifier == "EmergencyDetected" && e.DistanceMeters < 500, priority: 20),
                red);
        }

        foreach (var state in new[] { red, green, orange, orangeFlashing })
        {
            state.AddPath(new Trigger<TrafficEvent>(
                    e => e.Identifier == "EmergencyDetected" && e.DistanceMeters < 500, priority: 20),
                red);
        }

        red.AddPath(new Trigger<TrafficEvent>(
            e => e.Identifier == "EmergencyCleared", priority: 5), green);

        machine.Configure(new StateMachineConfig<TrafficEvent>(red));
        machine.Start();

        // TODO make it run as it should in an worker
        for (int i = 0; i < int.MaxValue; i++)
        {
            Thread.Sleep(100);

            if (i > 100)
            {
                machine.HandleEvent(new TrafficEvent("EmergencyDetected", 1000));
            }
        }
    }
}