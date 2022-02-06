namespace IS.Reading.Parsing.ArgumentParsers;

public interface IBackgroundImageTextParser
{
    Result<string> Parse(string value);
}
