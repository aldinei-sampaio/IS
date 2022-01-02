namespace IS.Reading.Conditions;

internal static class Helper
{
    public static bool AreEqual(object? value1, object? value2)
    {
        if (value1 is null)
            return value2 is null;
        if (value2 is null)
            return false;
        return value1.Equals(value2);
    }

    public static void WriteValues(TextWriter textWriter, IEnumerable<IConditionKeyword> values)
    {
        using var e = values.GetEnumerator();

        if (!e.MoveNext())
            return;

        e.Current.WriteTo(textWriter);
        while (e.MoveNext())
        {
            textWriter.Write(", ");
            e.Current.WriteTo(textWriter);
        }
    }
}
