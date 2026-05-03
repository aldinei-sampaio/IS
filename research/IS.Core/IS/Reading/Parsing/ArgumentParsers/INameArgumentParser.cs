namespace IS.Reading.Parsing.ArgumentParsers;

public interface INameArgumentParser
{
    Result<string> Parse(string value);
}
