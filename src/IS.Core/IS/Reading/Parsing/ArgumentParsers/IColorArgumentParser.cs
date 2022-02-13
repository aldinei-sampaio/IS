namespace IS.Reading.Parsing.ArgumentParsers;

public interface IColorArgumentParser
{
    Result<string> Parse(string value);
}
