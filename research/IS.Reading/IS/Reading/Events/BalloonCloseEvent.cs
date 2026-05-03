namespace IS.Reading.Events;

public class BalloonCloseEvent : IBalloonCloseEvent
{
    public BalloonCloseEvent(BalloonType ballonType, bool isMainCharacter)
        => (BalloonType, IsMainCharacter) = (ballonType, isMainCharacter);

    public BalloonType BalloonType { get; }

    public bool IsMainCharacter { get; }

    public override string ToString()
        => $"{BalloonType.ToString().ToLower()}{Helper.MainCharacterSymbol(BalloonType, IsMainCharacter)} end";
}
