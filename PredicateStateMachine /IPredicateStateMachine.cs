namespace PredicateStateMachine;

public interface IPredicateStateMachine<TEvent> where TEvent : IEvent
{
    void AddStates(List<IStateNode<TEvent>> newStates);
    void AddPath(IStateNode<TEvent> sourceState, ITransition<TEvent> transition, IStateNode<TEvent> targetState);
    void AddTimeout(IStateNode<TEvent> sourceState, TimeoutConfiguration<TEvent> timeoutConfiguration);
    void Configure(IStateMachineConfig<TEvent> config);
    void Start();
    void Pause();
    void Resume();
    void HandleEvent(TEvent e);
    IStateNode<TEvent> GetCurrentState();
}