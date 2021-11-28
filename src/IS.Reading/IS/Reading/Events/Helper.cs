namespace IS.Reading.Events;

internal static class Helper
{
    public static string ProtagSymbol(bool IsProtagonist)
        => IsProtagonist ? "*" : string.Empty;
}
