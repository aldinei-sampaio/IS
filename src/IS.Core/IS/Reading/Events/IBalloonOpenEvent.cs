namespace IS.Reading.Events;

public interface IBalloonOpenEvent : IReadingEvent
{
    BalloonType BalloonType { get; }
    bool IsMainCharacter { get; }
}
