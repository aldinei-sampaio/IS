namespace IS.Reading.Conditions;

public interface IWritable
{
    void WriteTo(TextWriter textWriter);
    string? ToString()
    {
        var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}
