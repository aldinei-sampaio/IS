using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public abstract class BalloonChildNodeParserBase : IAggregateNodeParser
{
    private readonly IElementParser elementParser;
    private readonly IBalloonTextNodeParser childParser;

    public IElementParserSettings Settings { get; }

    public IElementParserSettings AggregationSettings { get; }

    public BalloonChildNodeParserBase(
        IElementParser elementParser,
        IBalloonTextNodeParser balloonTextNodeParser,
        IChoiceNodeParser choiceNodeParser
    )
    {
        this.elementParser = elementParser;
        this.childParser = balloonTextNodeParser;
        Settings = ElementParserSettings.AggregatedNonRepeat(balloonTextNodeParser);
        AggregationSettings = ElementParserSettings.Aggregated(choiceNodeParser);
    }

    public string Name => childParser.Name;
    public BalloonType BalloonType => childParser.BalloonType;

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new BalloonChildParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (myContext.ParsedText is null)
            return;

        if (reader.ReadState != ReadState.EndOfFile)
            await elementParser.ParseAsync(reader, parsingContext, myContext, AggregationSettings);

        var node = new BalloonTextNode(myContext.ParsedText, BalloonType, myContext.ChoiceNode);
        parentParsingContext.AddNode(node);
    }
}
