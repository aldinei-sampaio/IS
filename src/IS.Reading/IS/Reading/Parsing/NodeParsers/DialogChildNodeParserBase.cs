using IS.Reading.Nodes;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public abstract class DialogChildNodeParserBase : IAggregateNodeParser
{
    public IElementParser ElementParser { get; }
    public ITextSourceParser TextSourceParser { get; }
    public IElementParserSettings Settings { get; }

    public DialogChildNodeParserBase(IElementParser elementParser, ITextSourceParser textSourceParser, IChoiceNodeParser choiceNodeParser)
    {
        ElementParser = elementParser;
        TextSourceParser = textSourceParser;
        Settings = new ElementParserSettings.AggregatedNonRepeat(choiceNodeParser);
    }

    public bool IsArgumentRequired => true;

    public abstract string Name { get; }

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var result = TextSourceParser.Parse(reader.Argument);
        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return;
        }
        var textSource = result.Value;

        var balloonType = ((BalloonParsingContext)parentParsingContext).BalloonType;

        var myContext = new BalloonChildParsingContext(balloonType);
        await ElementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (!parsingContext.IsSuccess)
            return;

        var node = new BalloonTextNode(textSource, balloonType, myContext.ChoiceBuilder);
        parentParsingContext.AddNode(node);

        parsingContext.SceneContext.Reset();
    }
}
