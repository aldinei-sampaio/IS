using IS.Reading.Choices;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Parsing.NodeParsers.ChoiceParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class ChoiceNodeParser : IChoiceNodeParser
{
    public IElementParser ElementParser { get; }
    public INameArgumentParser NameArgumentParser { get; }
    public IElementParserSettings Settings { get; }

    public ChoiceNodeParser(
        IElementParser elementParser,
        INameArgumentParser nameTextParser,
        IChoiceTimeLimitNodeParser timeLimitNodeParser,
        IChoiceDefaultNodeParser defaultNodeParser,
        IChoiceRandomOrderNodeParser randomOrderNodeParser,
        IChoiceOptionNodeParser optionNodeParser,
        IChoiceIfNodeParser choiceIfNodeParser
    )
    {
        ElementParser = elementParser;
        NameArgumentParser = nameTextParser;
        Settings = new ElementParserSettings.Block(
            timeLimitNodeParser,
            defaultNodeParser,
            randomOrderNodeParser,
            optionNodeParser,
            choiceIfNodeParser
        );
    }

    public bool IsArgumentRequired => true;

    public string Name => "?";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var keyParsingResult = NameArgumentParser.Parse(reader.Argument);
        if (!keyParsingResult.IsOk)
        {
            parsingContext.LogError(reader, keyParsingResult.ErrorMessage);
            return;
        }

        var myContext = new BuilderParentParsingContext<IChoicePrototype>();
        await ElementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (!parsingContext.IsSuccess || myContext.Builders.Count == 0)
            return;

        var ctx = (BalloonChildParsingContext)parentParsingContext;
        ctx.ChoiceBuilder = new ChoiceBuilder(keyParsingResult.Value, myContext.Builders);
    }
}