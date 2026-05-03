namespace IS.Reading.Parsing.ArgumentParsers;

public interface IImageArgumentParser
{
    Result<string> Parse(string value);
}
