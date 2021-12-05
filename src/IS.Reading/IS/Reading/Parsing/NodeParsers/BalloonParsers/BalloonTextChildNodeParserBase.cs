using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public abstract class BalloonTextChildNodeParserBase : IAggregateNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public IElementParserSettings AggregationSettings { get; }

    public BalloonTextChildNodeParserBase(
        IElementParser elementParser,
        IBalloonTextParser balloonTextParser,
        IChoiceNodeParser choiceNodeParser
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(balloonTextParser);
        AggregationSettings = ElementParserSettings.Aggregated(choiceNodeParser);
    }

    public abstract string Name { get; }
    public abstract BalloonType BalloonType { get; }

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (myContext.ParsedText is null)
            return;

        var aggContext = new BalloonTextChildParentParsingContext();
        if (reader.ReadState != ReadState.EndOfFile)
            await elementParser.ParseAsync(reader, parsingContext, aggContext, AggregationSettings);

        var node = new BalloonTextNode(myContext.ParsedText, BalloonType, aggContext.ChoiceNode);
        parentParsingContext.AddNode(node);
    }
}
