namespace IS.Reading.Events;

public class TimedPauseEvent(TimeSpan duration) : ITimedPauseEvent
{
    public TimeSpan Duration { get; } = duration;

    public override string ToString()
        => $"pause: {Duration.TotalMilliseconds}";
}
