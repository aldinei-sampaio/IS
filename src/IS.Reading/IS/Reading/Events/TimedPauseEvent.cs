namespace IS.Reading.Events;

public class TimedPauseEvent : ITimedPauseEvent
{
    public TimedPauseEvent(TimeSpan duration)
        => Duration = duration;

    public TimeSpan Duration { get; }

    public override string ToString()
        => $"pause: {Duration.TotalMilliseconds}";
}
