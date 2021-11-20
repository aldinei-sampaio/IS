using System.Xml;

namespace IS.Reading.Parsing.Text;

public interface ITextParser
{
    string? Parse(XmlReader reader, IParsingContext parsingContext, string value);
}
