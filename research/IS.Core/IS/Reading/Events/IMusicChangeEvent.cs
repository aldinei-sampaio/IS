namespace IS.Reading.Events;

public interface IMusicChangeEvent : IReadingEvent
{
    string? MusicName { get; }
}
