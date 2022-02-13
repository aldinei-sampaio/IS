using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceIfNodeParser : IChoiceIfNodeParser
{
    private readonly IElementParser elementParser;
    private readonly IConditionParser conditionParser;

    public IElementParserSettings IfBlockSettings { get; }
    public IElementParserSettings ElseBlockSettings { get; }

    public ChoiceIfNodeParser(
        IElementParser elementParser,
        IConditionParser conditionParser,
        IChoiceDefaultNodeParser defaultNodeParser,
        IChoiceRandomOrderNodeParser randomOrderNodeParser,
        IChoiceTimeLimitNodeParser timeLimitNodeParser,
        IChoiceOptionNodeParser optionNodeParser
    )
    {
        this.elementParser = elementParser;
        this.conditionParser = conditionParser;

        IfBlockSettings = new ElementParserSettings.IfBlock(
            defaultNodeParser,
            randomOrderNodeParser,
            timeLimitNodeParser,
            optionNodeParser,
            this
        );

        ElseBlockSettings = new ElementParserSettings.Block(
            defaultNodeParser,
            randomOrderNodeParser,
            timeLimitNodeParser,
            optionNodeParser,
            this
        );
    }

    public bool IsArgumentRequired => true;

    public string Name => "if";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (string.IsNullOrEmpty(reader.Argument))
            throw new InvalidOperationException();

        var parsingResult = conditionParser.Parse(reader.Argument);
        if (!parsingResult.IsOk)
        {
            parsingContext.LogError(reader, parsingResult.ErrorMessage);
            return;
        }

        var ifContext = new ChoiceParentParsingContext();
        var elseContext = new ChoiceParentParsingContext();

        await elementParser.ParseAsync(reader, parsingContext, ifContext, IfBlockSettings);

        if (!parsingContext.IsSuccess)
            return;

        if (!reader.AtEnd && string.Compare(reader.Command, "else", true) == 0)
        {
            await elementParser.ParseAsync(reader, parsingContext, elseContext, ElseBlockSettings);
            if (!parsingContext.IsSuccess)
                return;
        }

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.AddDecision(parsingResult.Value, ifContext.Builders, elseContext.Builders);
    }
}
