namespace IS.Reading.Parsing.ArgumentParsers;

public interface IColorTextParser
{
    Result<string> Parse(string value);
}
