namespace PredicateStateMachine;

using System;

public class PredicateStateMachine<TEvent> where TEvent : IEvent
{
    private IStateNode<TEvent> _current;
    private IStateNode<TEvent> _root;
    
    public void Configure(IStateMachineConfig<TEvent> config)
    {
        _root = config.GetRoot();
    }
    
    public void HandleEvent(TEvent e)
    {
        if (_current == null)
            return;
        _current = _current.HandleEvent(e);
    }

    public void Start()
    {
        if (_root == null)
            throw new Exception("Configuration error");
        _current = _root;
        _current.Start();
    }

    //hide this
    internal void Transition(IStateNode<TEvent> next)
    {
        _current = next;
    }

    public IStateNode<TEvent> GetCurrentState()
    {
        return _current;
    }
}