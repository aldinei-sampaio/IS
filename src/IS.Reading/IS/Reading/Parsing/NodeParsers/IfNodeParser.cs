using IS.Reading.Nodes;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class IfNodeParser : IIfNodeParser
{
    public IfNodeParser(
        IElementParser elementParser, 
        IConditionParser conditionParser, 
        IElementParserSettingsFactory elementParserSettingsFactory
    )
    {
        ElementParser = elementParser;
        ConditionParser = conditionParser;
        ElementParserSettingsFactory = elementParserSettingsFactory;
    }

    public bool IsArgumentRequired => true;
    public string Name => "if";
    public IElementParser ElementParser { get; }
    public IConditionParser ConditionParser { get; }
    public IElementParserSettingsFactory ElementParserSettingsFactory { get; }

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var elseContext = new ParentParsingContext();
        var decisionBlocks = new List<IDecisionBlock>();
        while (true)
        {
            var parsingResult = ConditionParser.Parse(reader.Argument);
            if (!parsingResult.IsOk)
            {
                parsingContext.LogError(reader, parsingResult.ErrorMessage);
                return;
            }

            var ifContext = new ParentParsingContext();
            await ElementParser.ParseAsync(reader, parsingContext, ifContext, ElementParserSettingsFactory.IfBlock);
            if (!parsingContext.IsSuccess)
                return;

            decisionBlocks.Add(new DecisionBlock(parsingResult.Value, parsingContext.BlockFactory.Create(ifContext.Nodes)));

            if (reader.AtEnd)
                break;

            var command = reader.Command;

            if (string.Compare(command, "elseif", true) != 0)
            {
                if (string.Compare(command, "else", true) == 0)
                {
                    await ElementParser.ParseAsync(reader, parsingContext, elseContext, ElementParserSettingsFactory.Block);
                    if (!parsingContext.IsSuccess)
                        return;
                }
                break;
            }
        }

        if (decisionBlocks.All(i => i.Block.Nodes.Count == 0) && elseContext.Nodes.Count == 0)
            return;

        var elseBlock = parsingContext.BlockFactory.Create(elseContext.Nodes);

        parentParsingContext.AddNode(new IfNode(decisionBlocks, elseBlock));
    }
}
