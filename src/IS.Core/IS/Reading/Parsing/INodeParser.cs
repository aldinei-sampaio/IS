using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing;

public interface INodeParser : IParser
{
    Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext);
    INode? DismissNode => null;
}
