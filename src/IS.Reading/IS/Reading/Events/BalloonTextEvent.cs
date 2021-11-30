namespace IS.Reading.Events;

public class BalloonTextEvent : IBalloonTextEvent
{
    public BalloonTextEvent(string text, BalloonType ballonType, bool isProtagonist)
        => (Text, BalloonType, IsProtagonist) = (text, ballonType, isProtagonist);

    public string Text { get; }

    public BalloonType BalloonType { get; }

    public bool IsProtagonist { get; }

    public override string ToString()
        => $"{BalloonType.ToString().ToLower()}{Helper.ProtagSymbol(BalloonType, IsProtagonist)}: {Text}";
}
