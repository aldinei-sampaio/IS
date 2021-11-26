using System.Xml;

namespace IS.Reading.Parsing;

public interface IAttributeParser : IParser
{
    IAttribute? Parse(XmlReader reader, IParsingContext parsingContext);
}
