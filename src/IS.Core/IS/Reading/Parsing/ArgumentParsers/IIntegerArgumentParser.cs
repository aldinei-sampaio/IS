namespace IS.Reading.Parsing.ArgumentParsers;

public interface IIntegerArgumentParser
{
    Result<int> Parse(string value, int minValue, int maxValue);
}
