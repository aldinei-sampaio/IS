using IS.Reading.Choices;

namespace IS.Reading.Events;

public class BalloonTextEvent : IBalloonTextEvent
{
    public BalloonTextEvent(string text, BalloonType ballonType, bool isMainCharacter, IChoice? choice)
        => (Text, BalloonType, IsMainCharacter, Choice) = (text, ballonType, isMainCharacter, choice);

    public string Text { get; }

    public BalloonType BalloonType { get; }

    public bool IsMainCharacter { get; }

    public IChoice? Choice { get; }

    public override string ToString()
        => $"{BalloonType.ToString().ToLower()}{Helper.MainCharacterSymbol(BalloonType, IsMainCharacter)}: {Text}";
}
