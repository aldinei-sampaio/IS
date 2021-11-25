using IS.Reading.State;

namespace IS.Reading.Events;

public class BackgroundScrollEvent : IBackgroundScrollEvent
{
    public BackgroundScrollEvent(BackgroundPosition position)
    { 
        if (position == BackgroundPosition.Undefined)
            throw new ArgumentOutOfRangeException(nameof(position));
        Position = position;
    }

    public BackgroundPosition Position { get; }

    public override string ToString()
        => "bg scroll";
}
