namespace IS.Reading.Events;

public interface IBalloonTextEvent : IReadingEvent
{
    BalloonType BallonType { get; }
    bool IsProtagonist { get; }
    string Text { get; }
}
