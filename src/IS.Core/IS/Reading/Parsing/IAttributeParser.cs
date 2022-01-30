using System.Xml;

namespace IS.Reading.Parsing;

public interface IAttributeParser : IParser
{
    IAttribute? Parse(IDocumentReader reader, IParsingContext parsingContext);
}
