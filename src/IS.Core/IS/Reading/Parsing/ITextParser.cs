using System.Xml;

namespace IS.Reading.Parsing;

public interface ITextParser
{
    string? Parse(XmlReader reader, IParsingContext parsingContext, string value);
}
