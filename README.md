# PredicateStateMachine
Lightweight C# state machine with timeout support, lifecycle hooks, and predicate transitions.
Focused on expressiveness and experimentation rather than production robustness (that will be done in a later stage.).

## Example

```csharp

var machine = new PredicateStateMachine<SensorEvent>(logger);

var idle = new SensorState("Idle");
var detected = new SensorState("Detected");
var alarm = new SensorState("Alarm")
{
    OnStartAction = () => Console.WriteLine("🚨 Alarm triggered!")
};

machine.AddPath(idle, new Transition<SensorEvent>(e => e is { Identifier: "MovementDetected" }), detected);
machine.AddPath(detected, new Transition<SensorEvent>(e => e is { Identifier: "MovementCleared" }), idle);
machine.AddTimeout(detected, new TimeoutConfiguration<SensorEvent>(
    timeoutMs: 5000,
    timeoutEvent: new SensorEvent("Timeout")
));
machine.AddPath(detected, new Transition<SensorEvent>(e => e is { Identifier: "Timeout" }), alarm);

machine.AddStates([idle, detected, alarm]);
machine.Configure(new StateMachineConfig<SensorEvent>(idle));
machine.Start();

machine.HandleEvent(new SensorEvent("MovementDetected"));
Thread.Sleep(3000);
machine.HandleEvent(new SensorEvent("MovementCleared"));
machine.HandleEvent(new SensorEvent("MovementDetected"));
machine.Pause();
machine.Resume();
Thread.Sleep(6000);

```


```csharp
→ Idle
12:37:47 info: PredicateStateMachine.PredicateStateMachine[0]
      [TRANSITION] {
        "OccurredAt": "2025-06-12T10:37:47.664908Z",
        "SourceStateName": "",
        "CauseDescription": "#MachineStarted",
        "TargetStateName": "Idle"
      }
→ Detected
12:37:47 info: PredicateStateMachine.PredicateStateMachine[0]
      [TRANSITION] {
        "OccurredAt": "2025-06-12T10:37:47.720936Z",
        "SourceStateName": "Idle",
        "CauseDescription": "MovementDetected",
        "TargetStateName": "Detected"
      }
→ Idle
12:37:50 info: PredicateStateMachine.PredicateStateMachine[0]
      [TRANSITION] {
        "OccurredAt": "2025-06-12T10:37:50.729388Z",
        "SourceStateName": "Detected",
        "CauseDescription": "MovementCleared",
        "TargetStateName": "Idle"
      }
→ Detected
12:37:50 info: PredicateStateMachine.PredicateStateMachine[0]
      [TRANSITION] {
        "OccurredAt": "2025-06-12T10:37:50.729852Z",
        "SourceStateName": "Idle",
        "CauseDescription": "MovementDetected",
        "TargetStateName": "Detected"
      }
12:37:50 info: PredicateStateMachine.PredicateStateMachine[0]
      [TRANSITION] {
        "OccurredAt": "2025-06-12T10:37:50.731134Z",
        "SourceStateName": "Detected",
        "CauseDescription": "#MachinePaused",
        "TargetStateName": ""
      }
→ Detected
12:37:50 info: PredicateStateMachine.PredicateStateMachine[0]
      [TRANSITION] {
        "OccurredAt": "2025-06-12T10:37:50.732291Z",
        "SourceStateName": null,
        "CauseDescription": "#MachineResumed",
        "TargetStateName": "Detected"
      }
→ Alarm
12:37:55 info: PredicateStateMachine.PredicateStateMachine[0]
      [TRANSITION] {
        "OccurredAt": "2025-06-12T10:37:55.737941Z",
        "SourceStateName": "Detected",
        "CauseDescription": "Timeout",
        "TargetStateName": "Alarm"
      }
Alaaaarm!

```
