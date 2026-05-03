namespace IS.Reading.Parsing;

public interface IParser
{
    public string Name { get; }
    public string? NameRegex => null;
}
