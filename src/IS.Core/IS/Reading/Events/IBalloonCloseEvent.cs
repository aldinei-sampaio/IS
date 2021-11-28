namespace IS.Reading.Events;

public interface IBalloonCloseEvent : IReadingEvent
{
    BalloonType BallonType { get; }
    bool IsProtagonist { get; }
}
