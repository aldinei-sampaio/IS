using System.Xml;

namespace IS.Reading.Parsing.Attributes
{
    public interface IAttributeParser
    {
        string ElementName { get; }
        IAttribute? Parse(XmlReader reader, IParsingContext parsingContext);
    }
}
