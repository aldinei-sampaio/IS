using System.Xml;

namespace IS.Reading.Parsing;

public interface IElementParser
{
    Task<IElementParsedData> ParseAsync(XmlReader reader, IParsingContext parsingContext, IElementParserSettings settings);
}
