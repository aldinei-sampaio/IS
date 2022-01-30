namespace IS.Reading.Parsing;

public interface ITextParser
{
    string? Parse(IDocumentReader reader, IParsingContext parsingContext, string value);
}
