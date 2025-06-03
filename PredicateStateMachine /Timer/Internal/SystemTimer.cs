using System.Timers;

namespace PredicateStateMachine.Timer.Impl;

internal class SystemTimer : ITimer
{
    private readonly System.Timers.Timer _timer;

    public SystemTimer(double interval)
    {
        _timer = new System.Timers.Timer(interval);
    }

    public double Interval { get => _timer.Interval; set => _timer.Interval = value; }
    public bool AutoReset { get => _timer.AutoReset; set => _timer.AutoReset = value; }

    public event ElapsedEventHandler Elapsed
    {
        add => _timer.Elapsed += value;
        remove => _timer.Elapsed -= value;
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();
    public void Dispose() => _timer.Dispose();
}