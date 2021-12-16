namespace IS.Reading.Events;

public class BalloonCloseEvent : IBalloonCloseEvent
{
    public BalloonCloseEvent(BalloonType ballonType, bool isProtagonist)
        => (BalloonType, IsProtagonist) = (ballonType, isProtagonist);

    public BalloonType BalloonType { get; }

    public bool IsProtagonist { get; }

    public override string ToString()
        => $"{BalloonType.ToString().ToLower()}{Helper.ProtagSymbol(BalloonType, IsProtagonist)} end";
}
