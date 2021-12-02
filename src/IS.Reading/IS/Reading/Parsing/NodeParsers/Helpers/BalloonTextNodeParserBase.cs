using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.Helpers;

public abstract class BalloonTextNodeParserBase : INodeParser
{
    public INodeParser ChildParser { get; protected set; }
    private readonly BalloonType balloonType;

    public BalloonTextNodeParserBase(
        string name,
        BalloonType balloonType,
        IElementParser elementParser,
        IBalloonTextParser balloonTextParser)
    {
        this.balloonType = balloonType;

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
        => new BalloonNode(balloonType, block);

    public Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
        => ChildParser.ParseAsync(reader, parsingContext);
}
