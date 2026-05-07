using IS.Reading.State;

namespace IS.Reading.Events;

public interface IBackgroundChangeEvent : IReadingEvent
{
    IBackgroundState State { get; }
    BackgroundAnimation Animation { get; }
    string? FlashColor { get; }
}
