namespace IS.Reading.Events;

public interface ITimedPauseEvent : IReadingEvent
{
    TimeSpan Duration { get; }
}
