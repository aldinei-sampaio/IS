namespace IS.Reading.Events;

public interface IBalloonCloseEvent : IReadingEvent
{
    BalloonType BalloonType { get; }
    bool IsProtagonist { get; }
}
