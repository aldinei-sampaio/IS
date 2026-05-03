using IS.Reading.State;

namespace IS.Reading.Events;

public interface IBackgroundScrollEvent : IReadingEvent
{
    BackgroundPosition Position { get;  }
}
