using IS.Reading.Choices;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public abstract class BuilderIfNodeParserBase<T> : INodeParser
{
    public IElementParser ElementParser { get; protected set; } = default!;
    public IConditionParser ConditionParser { get; protected set; } = default!;
    public IElementParserSettings IfBlockSettings { get; protected set; } = default!;
    public IElementParserSettings ElseBlockSettings { get; protected set; } = default!;

    public bool IsArgumentRequired => true;

    public string Name => "if";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var elseContext = new BuilderParentParsingContext<T>();
        var items = new List<IBuilderDecisionItem<T>>();
        for (; ; )
        {
            var parsingResult = ConditionParser.Parse(reader.Argument);
            if (!parsingResult.IsOk)
            {
                parsingContext.LogError(reader, parsingResult.ErrorMessage);
                return;
            }

            var ifContext = new BuilderParentParsingContext<T>();
            await ElementParser.ParseAsync(reader, parsingContext, ifContext, IfBlockSettings);
            if (!parsingContext.IsSuccess)
                return;

            items.Add(new BuilderDecisionItem<T>(parsingResult.Value, ifContext.Builders));

            if (reader.AtEnd)
                break;

            var command = reader.Command;

            if (string.Compare(command, "elseif", true) != 0)
            {
                if (string.Compare(command, "else", true) == 0)
                {
                    await ElementParser.ParseAsync(reader, parsingContext, elseContext, ElseBlockSettings);
                    if (!parsingContext.IsSuccess)
                        return;
                }
                break;
            }
        }

        if (!items.Any(i => i.Block.Any()) && !elseContext.Builders.Any())
            return;

        var ctx = (BuilderParentParsingContext<T>)parentParsingContext;
        ctx.Builders.Add(new BuilderDecision<T>(items, elseContext.Builders));
    }
}