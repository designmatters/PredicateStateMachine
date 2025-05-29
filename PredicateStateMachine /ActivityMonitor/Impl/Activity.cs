namespace PredicateStateMachine.ActivityMonitor.Impl;

internal class Activity<TEvent> where TEvent : IEvent
{
    public DateTime OccurredAt { get; }
    public string SourceStateName { get; }
    public string CauseDescription { get; }
    public string TargetStateName { get; }

    public Activity(
        DateTime occurredAt,
        string sourceStateName,
        string causeDescription,
        string targetStateName)
    {
        OccurredAt = occurredAt;
        SourceStateName = sourceStateName;
        CauseDescription = causeDescription;
        TargetStateName = targetStateName;
    }
}