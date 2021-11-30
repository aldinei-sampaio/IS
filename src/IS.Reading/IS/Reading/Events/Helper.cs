namespace IS.Reading.Events;

internal static class Helper
{
    public static string ProtagSymbol(BalloonType balloonType, bool isProtagonist)
    { 
        switch (balloonType)
        {
            case BalloonType.Speech:
            case BalloonType.Thought:
                return ProtagSymbol(isProtagonist);
            default:
                return string.Empty;
        }
    }

    public static string ProtagSymbol(bool isProtagonist)
        => isProtagonist ? "*" : string.Empty;
}
