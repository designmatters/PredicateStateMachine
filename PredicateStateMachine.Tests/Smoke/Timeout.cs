using System.Configuration;
using System.Threading;
using PredicateStateMachine;
using FluentAssertions;
using Xunit;

namespace PredicateStateMachine.Tests.Smoke;
    
public class Timeout
{
    [Fact]
    public void When_Timeout_Configured_State_Should_Timout()
    {
        var machine = new PredicateStateMachine<TestEvent>();

        var idle = new TestState(name: "idle");
        var active = new TestState(name: "active");

        machine.AddStates([idle, active]);
        
        machine.AddTimeout(idle, new TimeoutConfiguration<TestEvent>(100, new TestEvent("go")));
        machine.AddPath(idle, new Transition<TestEvent>(x => x.Identifier == "go"), active);

        machine.Configure(new StateMachineConfig<TestEvent>(idle));

        machine.Start();

        machine.GetCurrentState().Should().Be(idle);
        Thread.Sleep(50);
        machine.GetCurrentState().Should().Be(idle);
        Thread.Sleep(50);
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

