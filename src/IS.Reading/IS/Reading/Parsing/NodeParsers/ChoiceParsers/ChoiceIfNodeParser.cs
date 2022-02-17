using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceIfNodeParser : IChoiceIfNodeParser
{
    public IElementParser ElementParser { get; }
    public IConditionParser ConditionParser { get; }
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
        ElementParser = elementParser;
        ConditionParser = conditionParser;

        var parsers = new INodeParser[]
        {
            defaultNodeParser,
            randomOrderNodeParser,
            timeLimitNodeParser,
            optionNodeParser,
            this
        };

        IfBlockSettings = new ElementParserSettings.IfBlock(parsers);
        ElseBlockSettings = new ElementParserSettings.Block(parsers);
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

        var ifContext = new ChoiceParentParsingContext();
        var elseContext = new ChoiceParentParsingContext();

        await ElementParser.ParseAsync(reader, parsingContext, ifContext, IfBlockSettings);

        if (!parsingContext.IsSuccess || (ifContext.Builders.Count == 0 && elseContext.Builders.Count == 0))
            return;

        if (!reader.AtEnd && string.Compare(reader.Command, "else", true) == 0)
        {
            await ElementParser.ParseAsync(reader, parsingContext, elseContext, ElseBlockSettings);
            if (!parsingContext.IsSuccess)
                return;
        }

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.AddDecision(parsingResult.Value, ifContext.Builders, elseContext.Builders);
    }
}
