namespace IS.Reading.Events;

public class BalloonOpenEvent : IBalloonOpenEvent
{
    public BalloonOpenEvent(BalloonType ballonType, bool isProtagonist)
        => (BallonType, IsProtagonist) = (ballonType, isProtagonist);

    public BalloonType BallonType { get; }

    public bool IsProtagonist { get; }

    public override string ToString()
        => $"{BallonType.ToString().ToLower()}{Helper.ProtagSymbol(BallonType, IsProtagonist)} start";
}
