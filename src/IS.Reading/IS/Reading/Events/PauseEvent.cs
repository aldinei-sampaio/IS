namespace IS.Reading.Events;

public class PauseEvent : IPauseEvent
{
    public PauseEvent(TimeSpan? duration)
        => Duration = duration;

    public TimeSpan? Duration { get; }
}
