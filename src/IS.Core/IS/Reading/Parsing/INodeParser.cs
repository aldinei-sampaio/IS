using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing;

public delegate INode? NodeParserAggregator(IBlock block);

public interface INodeParser : IParser
{
    Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext);
    INode? DismissNode => null;
    INodeAggregation? NodeAggregation => null;
    INode? Aggregate(IBlock block) => null;
}
