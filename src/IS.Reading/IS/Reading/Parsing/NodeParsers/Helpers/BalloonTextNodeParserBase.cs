using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.Helpers;

public abstract class BalloonTextNodeParserBase : INodeParser
{
    public BalloonTextChildNodeParser ChildParser { get; }

    public BalloonTextNodeParserBase(
        string name,
        BalloonType balloonType,
        IElementParser elementParser,
        IBalloonTextParser balloonTextParser)
    {
        ChildParser = new BalloonTextChildNodeParser(
            elementParser, 
            balloonTextParser, 
            balloonType, 
            name
        );

        Aggregation = new NodeAggregation(ChildParser);
    }

    public string Name => ChildParser.Name;

    public INodeAggregation? Aggregation { get; }

    public INode? Aggregate(IBlock block)
        => new BalloonNode(ChildParser.BalloonType, block);

    public Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
        => ChildParser.ParseAsync(reader, parsingContext);
}
