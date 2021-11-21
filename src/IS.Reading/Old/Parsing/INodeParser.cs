using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing;

public interface INodeParser
{
    string ElementName { get; }
    Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext);
}
