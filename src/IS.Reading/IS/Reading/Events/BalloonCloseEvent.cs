namespace IS.Reading.Events;

public class BalloonCloseEvent : IBalloonCloseEvent
{
    public BalloonCloseEvent(BalloonType ballonType, bool isProtagonist)
        => (BallonType, IsProtagonist) = (ballonType, isProtagonist);

    public BalloonType BallonType { get; }

    public bool IsProtagonist { get; }

    public override string ToString()
        => $"{BallonType.ToString().ToLower()}{Helper.ProtagSymbol(IsProtagonist)} end";
}
