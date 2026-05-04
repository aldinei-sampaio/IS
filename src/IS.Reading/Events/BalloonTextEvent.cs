using IS.Reading.Choices;

namespace IS.Reading.Events;

public class BalloonTextEvent(string text, BalloonType balloonType, bool isMainCharacter, IChoice? choice) : IBalloonTextEvent
{
    public string Text { get; } = text;

    public BalloonType BalloonType { get; } = balloonType;

    public bool IsMainCharacter { get; } = isMainCharacter;

    public IChoice? Choice { get; } = choice;

    public override string ToString()
        => $"{BalloonType.ToString().ToLower()}{Helper.MainCharacterSymbol(BalloonType, IsMainCharacter)}: {Text}";
}
