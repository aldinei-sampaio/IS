using IS.Reading.Nodes;
using IS.Reading.Variables;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public abstract class BalloonChildNodeParserBase : IAggregateNodeParser
{
    private readonly IElementParser elementParser;
    private readonly ITextSourceParser textSourceParser;
    private readonly IBalloonTextNodeParser childParser;

    public IElementParserSettings Settings { get; }

    public IElementParserSettings AggregationSettings { get; }

    public BalloonChildNodeParserBase(
        IElementParser elementParser,
        ITextSourceParser textSourceParser,
        IBalloonTextNodeParser balloonTextNodeParser,
        IChoiceNodeParser choiceNodeParser
    )
    {
        this.elementParser = elementParser;
        this.textSourceParser = textSourceParser;
        this.childParser = balloonTextNodeParser;
        Settings = ElementParserSettings.AggregatedNonRepeat(balloonTextNodeParser);
        AggregationSettings = ElementParserSettings.AggregatedNonRepeat(choiceNodeParser);
    }

    public string Name => childParser.Name;
    public BalloonType BalloonType => childParser.BalloonType;

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new BalloonChildParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (myContext.ParsedText is null)
            return;

        var textSourceParsingResult = textSourceParser.Parse(myContext.ParsedText);
        if (textSourceParsingResult.IsError)
        {
            parsingContext.LogError(reader, textSourceParsingResult.ErrorMessage);
            return;
        }
        var textSource = textSourceParsingResult.TextSource;

        if (reader.ReadState != ReadState.EndOfFile)
            await elementParser.ParseAsync(reader, parsingContext, myContext, AggregationSettings);

        var node = new BalloonTextNode(textSource, BalloonType, myContext.ChoiceNode);
        parentParsingContext.AddNode(node);

        parsingContext.SceneContext.Reset();
    }
}
