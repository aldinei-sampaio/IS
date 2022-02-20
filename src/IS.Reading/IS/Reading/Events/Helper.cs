namespace IS.Reading.Events;

internal static class Helper
{
    public static string MainCharacterSymbol(BalloonType balloonType, bool isMainCharacter)
    { 
        switch (balloonType)
        {
            case BalloonType.Speech:
            case BalloonType.Thought:
                return MainCharacterSymbol(isMainCharacter);
            default:
                return string.Empty;
        }
    }

    public static string MainCharacterSymbol(bool isMainCharacter)
        => isMainCharacter ? "*" : string.Empty;
}
