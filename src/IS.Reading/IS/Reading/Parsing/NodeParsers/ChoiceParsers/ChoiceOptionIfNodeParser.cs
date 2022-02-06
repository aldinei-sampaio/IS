using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionIfNodeParser : IChoiceOptionIfNodeParser
{
    private readonly IElementParser elementParser;
    private readonly IConditionParser conditionParser;

    public IElementParserSettings IfBlockSettings { get; }
    public IElementParserSettings ElseBlockSettings { get; }

    public ChoiceOptionIfNodeParser(
        IElementParser elementParser,
        IConditionParser conditionParser,
        IChoiceOptionTextNodeParser choiceOptionTextNodeParser,
        IChoiceOptionDisabledNodeParser choiceOptionDisabledNodeParser,
        IChoiceOptionIconNodeParser choiceOptionIconNodeParser,
        IChoiceOptionTipNodeParser choiceOptionHelpTextNodeParser
    )
    {
        this.elementParser = elementParser;
        this.conditionParser = conditionParser;

        IfBlockSettings = ElementParserSettings.IfBlock(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionHelpTextNodeParser,
            this
        );

        ElseBlockSettings = ElementParserSettings.Block(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionHelpTextNodeParser,
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

        var ifContext = new ChoiceOptionParentParsingContext();
        var elseContext = new ChoiceOptionParentParsingContext();

        await elementParser.ParseAsync(reader, parsingContext, ifContext, IfBlockSettings);

        if (!parsingContext.IsSuccess)
            return;

        if (!reader.AtEnd && string.Compare(reader.ElementName, "else", true) == 0)
        {
            await elementParser.ParseAsync(reader, parsingContext, elseContext, ElseBlockSettings);
            if (!parsingContext.IsSuccess)
                return;
        }

        var ctx = (ChoiceOptionParentParsingContext)parentParsingContext;
        ctx.AddDecision(parsingResult.Value, ifContext.Builders, elseContext.Builders);
    }
}
