namespace IS.Reading.Conditions;

public abstract class WritableBase : IWritable
{
    public override string? ToString()
    {
        var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }

    public abstract void WriteTo(TextWriter textWriter);
}
