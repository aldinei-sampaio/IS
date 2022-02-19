using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionIfNodeParser : IChoiceOptionIfNodeParser
{
    public IElementParser ElementParser { get; }
    public IConditionParser ConditionParser { get; }
    public IElementParserSettings IfBlockSettings { get; }
    public IElementParserSettings ElseBlockSettings { get; }

    public ChoiceOptionIfNodeParser(
        IElementParser elementParser,
        IConditionParser conditionParser,
        IChoiceOptionTextNodeParser choiceOptionTextNodeParser,
        IChoiceOptionDisabledNodeParser choiceOptionDisabledNodeParser,
        IChoiceOptionIconNodeParser choiceOptionIconNodeParser,
        IChoiceOptionTipNodeParser choiceOptionTipNodeParser
    )
    {
        this.ElementParser = elementParser;
        this.ConditionParser = conditionParser;

        IfBlockSettings = new ElementParserSettings.IfBlock(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionTipNodeParser,
            this
        );

        ElseBlockSettings = new ElementParserSettings.Block(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionTipNodeParser,
            this
        );
    }

    public bool IsArgumentRequired => true;

    public string Name => "if";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var parsingResult = ConditionParser.Parse(reader.Argument);
        if (!parsingResult.IsOk)
        {
            parsingContext.LogError(reader, parsingResult.ErrorMessage);
            return;
        }

        var ifContext = new ChoiceOptionParentParsingContext();
        var elseContext = new ChoiceOptionParentParsingContext();

        await ElementParser.ParseAsync(reader, parsingContext, ifContext, IfBlockSettings);

        if (!parsingContext.IsSuccess || (ifContext.Builders.Count == 0 && elseContext.Builders.Count == 0))
            return;

        if (!reader.AtEnd && string.Compare(reader.Command, "else", true) == 0)
        {
            await ElementParser.ParseAsync(reader, parsingContext, elseContext, ElseBlockSettings);
            if (!parsingContext.IsSuccess)
                return;
        }

        var ctx = (ChoiceOptionParentParsingContext)parentParsingContext;
        ctx.AddDecision(parsingResult.Value, ifContext.Builders, elseContext.Builders);
    }
}
