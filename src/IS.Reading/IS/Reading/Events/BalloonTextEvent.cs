namespace IS.Reading.Events;

public class BalloonTextEvent : IBalloonTextEvent
{
    public BalloonTextEvent(string text, BalloonType ballonType, bool isProtagonist)
        => (Text, BallonType, IsProtagonist) = (text, ballonType, isProtagonist);

    public string Text { get; }

    public BalloonType BallonType { get; }

    public bool IsProtagonist { get; }

    public override string ToString()
        => $"{BallonType.ToString().ToLower()}{Helper.ProtagSymbol(IsProtagonist)}: {Text}";
}
