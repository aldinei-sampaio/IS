namespace IS.Reading.Events;

public interface IPauseEvent : IReadingEvent
{
    TimeSpan? Duration { get; }
}
