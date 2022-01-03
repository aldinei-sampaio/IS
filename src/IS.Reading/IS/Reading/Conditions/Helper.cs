namespace IS.Reading.Conditions;

internal static class Helper
{
    public static bool AreEqual(object? value1, object? value2)
    {
        var v2 = value2 as IComparable;
        
        if (value1 is not IComparable v1)
            return v2 is null;

        if (v2 is null)
            return false;

        return v1.GetType() == v2.GetType() && v1.CompareTo(v2) == 0;
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
