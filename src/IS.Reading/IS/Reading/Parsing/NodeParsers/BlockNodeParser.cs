using IS.Reading.Conditions;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class BlockNodeParser : IBlockNodeParser
{
    private readonly IElementParser elementParser;
    private readonly IConditionParser conditionParser;

    public IElementParserSettings Settings { get; }

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

        Settings = ElementParserSettings.Normal(
            musicNodeParser,
            backgroundNodeParser,
            pauseNodeParser,
            protagonistNodeParser,
            personNodeParser,
            narrationNodeParser,
            tutorialNodeParser,
            setNodeParser
        );

        Settings.ChildParsers.Add(this);
    }

    public string Name => string.Empty;

    public string? NameRegex => "(if|while)";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (string.IsNullOrWhiteSpace(reader.Argument))
        {
            parsingContext.LogError(reader, "Era esperada uma condição");
            return;
        }

        var result = conditionParser.Parse(reader.Argument);

        if (result.Condition is null)
        {
            parsingContext.LogError(reader, result.Message);
            return;
        }

        var myContext = new ParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (myContext.Nodes.Count == 0)
        {
            parsingContext.LogError(reader, "Elemento filho era esperado.");
            return;
        }

        ICondition? when = null;
        ICondition? @while = null;

        if (string.Compare(reader.ElementName, "if", true) == 0)
            when = result.Condition;
        else
            @while = result.Condition;

        var block = parsingContext.BlockFactory.Create(myContext.Nodes, @while);
        var node = new BlockNode(block, @when);
        parentParsingContext.AddNode(node);
    }
}
