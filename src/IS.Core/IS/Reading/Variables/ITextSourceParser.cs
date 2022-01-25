namespace IS.Reading.Variables;

public interface ITextSourceParser
{
    public ITextSourceParserResult Parse(string text);
}
