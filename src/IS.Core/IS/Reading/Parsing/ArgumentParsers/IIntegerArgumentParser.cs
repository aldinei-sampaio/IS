namespace IS.Reading.Parsing.ArgumentParsers;

public interface IIntegerArgumentParser
{
    int? Parse(IDocumentReader reader, IParsingContext parsingContext, string value, int minValue, int maxValue);
}
