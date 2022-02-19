using IS.Reading.Conditions;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class BlockNodeParser : IBlockNodeParser
{
    public IElementParser ElementParser { get; }
    public IConditionParser ConditionParser { get; }
    public IElementParserSettings IfBlockSettings { get; }
    public IElementParserSettings BlockSettings { get; }

    public BlockNodeParser(
        IElementParser elementParser,
        IConditionParser conditionParser,
        IMusicNodeParser musicNodeParser,
        IBackgroundNodeParser backgroundNodeParser,
        IPauseNodeParser pauseNodeParser,
        IProtagonistNodeParser protagonistNodeParser,
        IPersonNodeParser personNodeParser,
        INarrationNodeParser narrationNodeParser,
        ITutorialNodeParser tutorialNodeParser,
        ISetNodeParser setNodeParser
    )
    {
        ElementParser = elementParser;
        ConditionParser = conditionParser;

        var nodeParsers = new INodeParser[]
        {
            musicNodeParser,
            backgroundNodeParser,
            pauseNodeParser,
            protagonistNodeParser,
            personNodeParser,
            narrationNodeParser,
            tutorialNodeParser,
            setNodeParser,
            this
        };

        IfBlockSettings = new ElementParserSettings.IfBlock(nodeParsers);
        BlockSettings = new ElementParserSettings.Block(nodeParsers);
    }

    public bool IsArgumentRequired => true;

    public string Name => string.Empty;

    public string? NameRegex => "(if|while)";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var result = ConditionParser.Parse(reader.Argument);

        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return;
        }

        if (string.Compare(reader.Command, "if", true) == 0)
            await ParseIfAsync(result.Value, reader, parsingContext, parentParsingContext);
        else
            await ParseWhileAsync(result.Value, reader, parsingContext, parentParsingContext);
    }

    private async Task ParseIfAsync(
        ICondition condition,
        IDocumentReader reader, 
        IParsingContext parsingContext, 
        IParentParsingContext parentParsingContext
    )
    {
        var ifContext = new ParentParsingContext();
        var elseContext = new ParentParsingContext();

        await ElementParser.ParseAsync(reader, parsingContext, ifContext, IfBlockSettings);

        if (!parsingContext.IsSuccess)
            return;

        if (!reader.AtEnd && string.Compare(reader.Command, "else", true) == 0)
        {
            await ElementParser.ParseAsync(reader, parsingContext, elseContext, BlockSettings);
            if (!parsingContext.IsSuccess)
                return;
        }

        var ifBlock = parsingContext.BlockFactory.Create(ifContext.Nodes);
        var elseBlock = parsingContext.BlockFactory.Create(elseContext.Nodes);

        parentParsingContext.AddNode(new IfNode(condition, ifBlock, elseBlock));
    }

    private async Task ParseWhileAsync(
        ICondition condition,
        IDocumentReader reader,
        IParsingContext parsingContext,
        IParentParsingContext parentParsingContext
    )
    {
        var myContext = new ParentParsingContext();

        await ElementParser.ParseAsync(reader, parsingContext, myContext, BlockSettings);

        if (!parsingContext.IsSuccess)
            return;

        var block = parsingContext.BlockFactory.Create(myContext.Nodes, condition);
        parentParsingContext.AddNode(new BlockNode(block));
    }
}
