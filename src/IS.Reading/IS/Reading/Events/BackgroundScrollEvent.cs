using IS.Reading.State;

namespace IS.Reading.Events;

public class BackgroundScrollEvent : IBackgroundScrollEvent
{
    public BackgroundScrollEvent(BackgroundPosition position)
        => Position = position;

    public BackgroundPosition Position { get; }
}
