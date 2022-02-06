namespace IS.Reading.Variables;

public interface ITextSourceParser
{
    Result<ITextSource> Parse(string value);
}
