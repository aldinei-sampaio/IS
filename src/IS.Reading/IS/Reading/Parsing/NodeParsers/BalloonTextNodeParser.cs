using IS.Reading.Nodes;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class BalloonTextNodeParser : IBalloonTextNodeParser
{
    public IElementParser ElementParser { get; }
    public ITextSourceParser TextSourceParser { get; }
    public IElementParserSettings Settings { get; }

    public BalloonTextNodeParser(IElementParser elementParser, ITextSourceParser textSourceParser, IChoiceNodeParser choiceNodeParser)
    {
        ElementParser = elementParser;
        TextSourceParser = textSourceParser;
        Settings = new ElementParserSettings.AggregatedNonRepeat(choiceNodeParser);
    }

    public bool IsArgumentRequired => true;

    public string Name => "-";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var balloonParentContext = (BalloonParsingContext)parentParsingContext;

        var result = TextSourceParser.Parse(reader.Argument);
        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return;
        }
        var textSource = result.Value;

        var myContext = new BalloonChildParsingContext(balloonParentContext.BalloonType);
        await ElementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (!parsingContext.IsSuccess)
            return;

        var node = new BalloonTextNode(textSource, balloonParentContext.BalloonType, myContext.ChoiceBuilder);
        parentParsingContext.AddNode(node);

        parsingContext.SceneContext.Reset();
    }
}

