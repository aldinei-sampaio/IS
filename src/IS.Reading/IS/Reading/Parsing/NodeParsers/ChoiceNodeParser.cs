using IS.Reading.Choices;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Parsing.NodeParsers.ChoiceParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class ChoiceNodeParser : IChoiceNodeParser
{
    private readonly IElementParser elementParser;
    private readonly INameTextParser nameTextParser;

    public IElementParserSettings Settings { get; }

    public ChoiceNodeParser(
        IElementParser elementParser,
        INameTextParser nameTextParser,
        IChoiceTimeLimitNodeParser timeLimitNodeParser,
        IChoiceDefaultNodeParser defaultNodeParser,
        IChoiceRandomOrderNodeParser randomOrderNodeParser,
        IChoiceOptionNodeParser optionNodeParser
    )
    {
        this.elementParser = elementParser;
        this.nameTextParser = nameTextParser;
        Settings = ElementParserSettings.Block(
            timeLimitNodeParser,
            defaultNodeParser,
            randomOrderNodeParser,
            optionNodeParser
        );
    }

    public bool IsArgumentRequired => true;

    public string Name => "?";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var keyParsingResult = nameTextParser.Parse(reader.Argument);
        if (!keyParsingResult.IsOk)
        {
            parsingContext.LogError(reader, keyParsingResult.ErrorMessage);
            return;
        }

        var myContext = new ChoiceParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (myContext.Builders.Count == 0)
        {
            parsingContext.LogError(reader, "Nenhuma opção informada.");
            return;
        }

        var ctx = (BalloonChildParsingContext)parentParsingContext;
        ctx.ChoiceBuilder = new ChoiceBuilder(keyParsingResult.Value, myContext.Builders);
    }
}