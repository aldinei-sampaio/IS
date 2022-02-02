using IS.Reading.Nodes;
using IS.Reading.Parsing.NodeParsers.BalloonParsers;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class BalloonTextNodeParser : IBalloonTextNodeParser
{
    private readonly IElementParser elementParser;
    private readonly ITextSourceParser textSourceParser;

    public IElementParserSettings Settings { get; }

    public BalloonTextNodeParser(IElementParser elementParser, ITextSourceParser textSourceParser, IChoiceNodeParser choiceNodeParser)
    {
        this.elementParser = elementParser;
        this.textSourceParser = textSourceParser;
        Settings = ElementParserSettings.AggregatedNonRepeat(choiceNodeParser);
    }

    public string Name => "-";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (parentParsingContext is not BalloonParsingContext balloonParentContext)
            throw new ArgumentException($"Argumento '{nameof(balloonParentContext)}' precisa ser do tipo '{nameof(BalloonParsingContext)}'.", nameof(parentParsingContext));

        if (string.IsNullOrWhiteSpace(reader.Argument))
        {
            parsingContext.LogError(reader, "Texto do diálogo não informado.");
            return;
        }

        var textSourceParsingResult = textSourceParser.Parse(reader.Argument);
        if (textSourceParsingResult.IsError)
        {
            parsingContext.LogError(reader, textSourceParsingResult.ErrorMessage);
            return;
        }
        var textSource = textSourceParsingResult.TextSource;

        var myContext = new BalloonChildParsingContext(balloonParentContext.BalloonType);
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var node = new BalloonTextNode(textSource, balloonParentContext.BalloonType, myContext.ChoiceNode);
        parentParsingContext.AddNode(node);

        parsingContext.SceneContext.Reset();
    }
}

