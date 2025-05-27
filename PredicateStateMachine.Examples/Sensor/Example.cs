using PredicateStateMachine;

namespace Sensor;

public static class Example
{
    public static void Run()
    {
        var machine = new PredicateStateMachine<SensorEvent>();

        var idle = new SensorState(machine, "Idle");
        var detected = new SensorState(machine, "Detected");
        var alarm = new SensorState(machine, "Alarm");

        idle.AddPath(new Trigger<SensorEvent>(e => e.Identifier == "MovementDetected"), detected);
        detected.AddPath(new Trigger<SensorEvent>(e => e.Identifier == "MovementCleared"), idle);
        detected.AddTimeout(new StateTimeoutConfiguration<SensorEvent>(
            timeoutMs: 5000,
            new SensorEvent("Timeout")));
        detected.AddPath(new Trigger<SensorEvent>(e => e.Identifier == "Timeout"), alarm);

        machine.Configure(new StateMachineConfig<SensorEvent>(idle));
        machine.Start();

        var movementDetected = new SensorEvent("MovementDetected");
        var movementCleared = new SensorEvent("MovementCleared");

        machine.HandleEvent(movementDetected);
        Thread.Sleep(3000);
        machine.HandleEvent(movementCleared);
        machine.HandleEvent(movementDetected);
        Thread.Sleep(6000);
    }
}