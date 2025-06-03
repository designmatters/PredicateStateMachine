using System.Configuration;
using System.Threading;
using System.Timers;
using PredicateStateMachine;
using FluentAssertions;
using Xunit;
using ITimer = PredicateStateMachine.Timer.ITimer;

namespace PredicateStateMachine.Tests;

public class Basics
{
    [Fact]
    public void Should_Transition_When_Trigger_Is_Matched()
    {
        var machine = new PredicateStateMachine<TestEvent>();

        var idle = new TestState(name: "idle");
        var active = new TestState(name: "active");

        machine.AddStates([idle, active]);

        machine.AddPath(idle, new Transition<TestEvent>(x => x.Identifier == "go"), active);

        machine.Configure(new StateMachineConfig<TestEvent>(idle));

        machine.Start();

        machine.GetCurrentState().Should().Be(idle); //get rid of

        machine.HandleEvent(new TestEvent("go"));

        machine.GetCurrentState().Should().Be(active);
    }

    [Fact]
    public void Should_Not_Transition_When_Trigger_Does_Not_Match()
    {
        var machine = new PredicateStateMachine<TestEvent>();

        var idle = new TestState(name: "idle");
        var active = new TestState(name: "active");

        machine.AddStates([idle, active]);

        machine.AddPath(idle, new Transition<TestEvent>(x => x.Identifier == "go"), active);

        machine.Configure(new StateMachineConfig<TestEvent>(idle));

        machine.Start();

        machine.HandleEvent(new TestEvent("no"));

        machine.GetCurrentState().Should().Be(idle);
    }


    [Fact]
    public void Should_Transition_When_Trigger_And_Predicate_Is_Matched()
    {
        var machine = new PredicateStateMachine<TestEvent>();

        var idle = new TestState(name: "idle");
        var active = new TestState(name: "active");

        machine.AddStates([idle, active]);

        machine.AddPath(idle, new Transition<TestEvent>(x => x.Identifier == "go", x => x.Value1 > 100), active);

        machine.Configure(new StateMachineConfig<TestEvent>(idle));

        machine.Start();

        machine.GetCurrentState().Should().Be(idle);

        machine.HandleEvent(new TestEvent("go") { Value1 = 130 });

        machine.GetCurrentState().Should().Be(active);
    }

    [Fact]
    public void Should_Not_Transition_When_Predicate_Does_Not_Match()
    {
        var machine = new PredicateStateMachine<TestEvent>();

        var idle = new TestState(name: "idle");
        var active = new TestState(name: "active");

        machine.AddStates([idle, active]);

        machine.AddPath(idle, new Transition<TestEvent>(x => x.Identifier == "go", x => x.Value1 > 100), active);

        machine.Configure(new StateMachineConfig<TestEvent>(idle));

        machine.Start();

        machine.GetCurrentState().Should().Be(idle);

        machine.HandleEvent(new TestEvent("go") { Value1 = 80 });

        machine.GetCurrentState().Should().Be(idle);
    }
    
    [Fact]
    public void When_Timeout_Configured_State_Should_Timeout() //TO FIX
    {
        var machine = new PredicateStateMachine<TestEvent>();

        var idle = new TestState(name: "idle");
        var active = new TestState(name: "active");

        machine.AddStates([idle, active]);

        var fakeTimer = new FakeTimer();
        
        machine.AddTimeout(idle, new TimeoutConfiguration<TestEvent>(100, 
            new TestEvent("go"), 
            () => new FakeTimer()));
        
        machine.AddPath(idle, new Transition<TestEvent>(x => x.Identifier == "go"), active);
        
        machine.Configure(new StateMachineConfig<TestEvent>(idle));
        
        machine.Start();
        machine.GetCurrentState().Should().Be(idle);
        fakeTimer.TriggerElapsed();
        machine.GetCurrentState().Should().Be(active);
    }

   
    
    private class TestEvent : IEvent
    {
        public string Identifier { get; set; }
        public double Timeout { get; set; }
        public int Value1 { get; set; }

        public TestEvent(string identifier)
        {
            Identifier = identifier;
        }
    }

    private class TestState : IStateNode<TestEvent>
    {
        public string Name { get; set; }

        public TestState(string name)
        {
            Name = name;
        }

        public void OnBeforeStart()
        {
        }

        public void OnStart()
        {
        }

        public void OnAfterStart()
        {
        }

        public void OnBeforeStop()
        {
        }

        public void OnStop()
        {
        }

        public void OnAfterStop()
        {
        }
    }
}

class FakeTimer : ITimer
{
    public event ElapsedEventHandler? Elapsed;
    public void Start() => Started = true;
    public void Stop() => Stopped = true;
    public void Dispose() { }
    public double Interval { get; set; }
    public bool AutoReset { get; set; }
    public bool Started { get; private set; }
    public bool Stopped { get; private set; }
    public void TriggerElapsed() => Elapsed?.Invoke(this, null);
}
