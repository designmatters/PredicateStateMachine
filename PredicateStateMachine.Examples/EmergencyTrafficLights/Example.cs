using PredicateStateMachine;

namespace EmergencyTrafficLight;

public static class Example
{
    public static async Task Run()
    {
        var machine = new PredicateStateMachine<TrafficEvent>();

        var red = new TrafficState("Red");
        var green = new TrafficState("Green");
        var orange = new TrafficState("Orange");
        var orangeFlashing = new TrafficState("OrangeFlashing");

        machine.AddTimeout(red, new StateTimeoutConfiguration<TrafficEvent>(5000, new TrafficEvent("Timer")));
        machine.AddPath(red, new Transition<TrafficEvent>(e => e.Identifier == "Timer"), green);

        machine.AddTimeout(green, new StateTimeoutConfiguration<TrafficEvent>(4000, new TrafficEvent("Timer")));
        machine.AddPath(green, new Transition<TrafficEvent>(e => e.Identifier == "Timer"), orange);

        machine.AddTimeout(orange, new StateTimeoutConfiguration<TrafficEvent>(2000, new TrafficEvent("Timer")));
        machine.AddPath(orange, new Transition<TrafficEvent>(e => e.Identifier == "Timer"), red);
        
        foreach (var state in new[] { red, green, orange })
        {
            machine.AddPath(state, new Transition<TrafficEvent>(
                e => e is { Identifier: "EmergencyDetected", DistanceMeters: 1000 }, priority: 10), orangeFlashing);
        }

        foreach (var state in new[] { red, green, orange, orangeFlashing })
        {
            machine.AddPath(state, new Transition<TrafficEvent>(
                e => e is { Identifier: "EmergencyDetected", DistanceMeters: < 500 }, priority: 20), red);
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
