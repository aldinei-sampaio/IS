namespace IS.Reading.Events;

public class BalloonCloseEvent(BalloonType balloonType, bool isMainCharacter) : IBalloonCloseEvent
{
    public BalloonType BalloonType { get; } = balloonType;

    public bool IsMainCharacter { get; } = isMainCharacter;

    public override string ToString()
        => $"{BalloonType.ToString().ToLower()}{Helper.MainCharacterSymbol(BalloonType, IsMainCharacter)} end";
}
