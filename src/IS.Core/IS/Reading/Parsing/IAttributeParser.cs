using System.Xml;

namespace IS.Reading.Parsing;

public interface IAttributeParser
{
    string ElementName { get; }
    IAttribute? Parse(XmlReader reader, IParsingContext parsingContext);
}
