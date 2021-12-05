using IS.Reading.Choices;

namespace IS.Reading.Events;

public interface IBalloonTextEvent : IReadingEvent
{
    BalloonType BalloonType { get; }
    bool IsProtagonist { get; }
    string Text { get; }
    IChoice? Choice { get; }
}
