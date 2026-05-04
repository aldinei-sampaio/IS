namespace IS.Reading.Events;

public class BalloonOpenEvent(BalloonType balloonType, bool isMainCharacter) : IBalloonOpenEvent
{
    public BalloonType BalloonType { get; } = balloonType;

    public bool IsMainCharacter { get; } = isMainCharacter;

    public override string ToString()
        => $"{BalloonType.ToString().ToLower()}{Helper.MainCharacterSymbol(BalloonType, IsMainCharacter)} start";
}
