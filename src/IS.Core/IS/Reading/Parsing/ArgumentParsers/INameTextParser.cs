namespace IS.Reading.Parsing.ArgumentParsers;

public interface INameTextParser
{
    Result<string> Parse(string value);
}
