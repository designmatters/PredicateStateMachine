using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PredicateStateMachine.ActivityMonitor.Impl;

internal class DefaultActivityMonitor<TEvent> : IActivityMonitor<TEvent>
    where TEvent : IEvent
{
    private readonly ILogger? _logger;
    private readonly List<Activity<TEvent>> _activities = new();

    public DefaultActivityMonitor(ILogger? logger = null)
    {
        _logger = logger;
    }

    public void Register(IStateNode<TEvent>? sourceState, IEvent e, IStateNode<TEvent> targetState)
    {
        var activity = new Activity<TEvent>(
            DateTime.UtcNow,
            sourceState?.Name ?? "",
            e.Identifier,
            targetState.Name);

        Flush(activity);
    }

    public void RegisterMachineStart(IStateNode<TEvent>? sourceState, IStateNode<TEvent> targetState)
    {
        var activity = new Activity<TEvent>(
            DateTime.UtcNow,
            sourceState?.Name ?? "",
            "#MachineStarted",
            targetState.Name);

        Flush(activity);
    }

    public void RegisterMachineStop(IStateNode<TEvent>? sourceState)
    {
        var activity = new Activity<TEvent>(
            DateTime.UtcNow,
            sourceState?.Name ?? "",
            "#MachineStopped",
            "");

        Flush(activity);
    }

    public IEnumerable<Activity<TEvent>> Query() => _activities.AsReadOnly();

    private void Flush(Activity<TEvent> activity)
    {
        _activities.Add(activity);
        var json = JsonSerializer.Serialize(activity, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        _logger?.LogInformation("[TRANSITION] {ActivityJson}", json);
    }
}