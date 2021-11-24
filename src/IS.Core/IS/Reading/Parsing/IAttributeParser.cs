using System.Xml;

namespace IS.Reading.Parsing;

public interface IAttributeParser
{
    string AttributeName { get; }
    IAttribute? Parse(XmlReader reader, IParsingContext parsingContext);
}
