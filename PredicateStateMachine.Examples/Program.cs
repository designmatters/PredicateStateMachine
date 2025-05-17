// See https://aka.ms/new-console-template for more information

using PredicateStateMachine;


var machine = new SimpleStateMachine<SensorEvent, object>();

var idle = new SensorState(machine, "Idle");
var detected = new SensorState(machine, "Detected") { TimeoutMs = 5000 };
var alarm = new SensorState(machine, "Alarm");

idle.AddPath(new SensorTrigger(SensorEvent.MovementDetected), detected);
detected.AddPath(new SensorTrigger(SensorEvent.MovementCleared), idle);
detected.AddPath(new SensorTrigger(SensorEvent.Timeout), alarm);

machine.Configure(new StateMachineConfig<SensorEvent, object>(idle));
machine.Start();

// Simulate events
machine.HandleEvent(SensorEvent.MovementDetected); // -> Detected
Thread.Sleep(3000);
machine.HandleEvent(SensorEvent.MovementCleared); // -> Idle before timeout

machine.HandleEvent(SensorEvent.MovementDetected); // -> Detected
Thread.Sleep(6000); // timeout fires → Alarm