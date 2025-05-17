namespace PredicateStateMachine;

using System;

public class SimpleStateMachine<TEvent, TData>
{
    private IStateNode<TEvent, TData> _current;
    private IStateNode<TEvent, TData> _root;


    public void Configure(IStateMachineConfig<TEvent, TData> config)
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
        _current.Start(null);
    }
}