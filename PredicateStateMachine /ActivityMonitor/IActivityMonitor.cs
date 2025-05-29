using PredicateStateMachine.ActivityMonitor.Impl;

namespace PredicateStateMachine.ActivityMonitor;

internal interface IActivityMonitor<TEvent> where TEvent : IEvent
{
    void Register(IStateNode<TEvent>? sourceState, IEvent e, IStateNode<TEvent> targetState);
    void RegisterMachineStart(IStateNode<TEvent>? sourceState, IStateNode<TEvent> targetState);
    void RegisterMachineStop(IStateNode<TEvent>? sourceState);
    IEnumerable<Activity<TEvent>> Query();
}