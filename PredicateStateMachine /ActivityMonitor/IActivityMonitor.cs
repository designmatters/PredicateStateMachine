using PredicateStateMachine.ActivityMonitor.Impl;

namespace PredicateStateMachine.ActivityMonitor;

internal interface IActivityMonitor<TEvent> where TEvent : IEvent
{
    void Register(IStateNode<TEvent>? sourceState, IEvent e, IStateNode<TEvent> targetState);
    void RegisterMachineStarted(IStateNode<TEvent>? sourceState, IStateNode<TEvent> targetState);
    void RegisterMachinePaused(IStateNode<TEvent>? sourceState);
    void RegisterMachineResumed(IStateNode<TEvent>? resumedOnState);
    IEnumerable<Activity<TEvent>> Query();
}