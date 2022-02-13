using IS.Reading.Conditions;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class BlockNodeParser : IBlockNodeParser
{
    private readonly IElementParser elementParser;
    private readonly IConditionParser conditionParser;

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
        this.elementParser = elementParser;
        this.conditionParser = conditionParser;

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
        var result = conditionParser.Parse(reader.Argument);

        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return;
        }

        if (string.Compare(reader.ElementName, "if", true) == 0)
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

        using (var subReader = reader.ReadSubtree())
            await elementParser.ParseAsync(subReader, parsingContext, ifContext, IfBlockSettings);

        if (!parsingContext.IsSuccess)
            return;
        
        if (!reader.AtEnd && string.Compare(reader.ElementName, "else", true) == 0)
            await elementParser.ParseAsync(reader, parsingContext, elseContext, BlockSettings);

        if (!parsingContext.IsSuccess)
            return;

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

        using (var subReader = reader.ReadSubtree())
            await elementParser.ParseAsync(subReader, parsingContext, myContext, BlockSettings);

        if (!parsingContext.IsSuccess)
            return;

        var block = parsingContext.BlockFactory.Create(myContext.Nodes, condition);
        parentParsingContext.AddNode(new BlockNode(block));
    }
}
