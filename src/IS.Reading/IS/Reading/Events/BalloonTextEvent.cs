using IS.Reading.Choices;

namespace IS.Reading.Events;

public class BalloonTextEvent : IBalloonTextEvent
{
    public BalloonTextEvent(string text, BalloonType ballonType, bool isProtagonist, IChoice? choice)
        => (Text, BalloonType, IsProtagonist, Choice) = (text, ballonType, isProtagonist, choice);

    public string Text { get; }

    public BalloonType BalloonType { get; }

    public bool IsProtagonist { get; }

    public IChoice? Choice { get; }

    public override string ToString()
        => $"{BalloonType.ToString().ToLower()}{Helper.ProtagSymbol(BalloonType, IsProtagonist)}: {Text}";
}
