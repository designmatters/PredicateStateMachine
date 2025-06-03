using System.Timers;

namespace PredicateStateMachine.Timer;

public interface ITimer : IDisposable
{
    void Start();
    void Stop();
    double Interval { get; set; }
    bool AutoReset { get; set; }
    event ElapsedEventHandler Elapsed;
}