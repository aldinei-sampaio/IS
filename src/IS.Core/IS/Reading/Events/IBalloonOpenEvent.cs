namespace IS.Reading.Events;

public interface IBalloonOpenEvent : IReadingEvent
{
    BalloonType BallonType { get; }
    bool IsProtagonist { get; }
}
