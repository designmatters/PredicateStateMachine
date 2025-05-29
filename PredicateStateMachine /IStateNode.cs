namespace PredicateStateMachine;

public interface IStateNode<TEvent> where TEvent : IEvent
{
    string Name { get; set; }
    void OnBeforeStart();
    void OnStart();
    void OnAfterStart();
    void OnBeforeStop();
    void OnStop();
    void OnAfterStop();
}