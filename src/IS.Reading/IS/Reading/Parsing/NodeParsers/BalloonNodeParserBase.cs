using IS.Reading.Navigation;
using IS.Reading.Nodes;

namespace IS.Reading.Parsing.NodeParsers;

public abstract class BalloonNodeParserBase : IAggregateNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public BalloonNodeParserBase(
        IElementParser elementParser,
        params object[] parsers
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Aggregated(parsers);
    }

    public abstract string Name { get; }

    public abstract BalloonType BalloonType { get; }

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new BalloonParsingContext(BalloonType);
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var nonPauseNodes = new Stack<INode>();
        for (var n = myContext.Nodes.Count - 1; n >= 0; n--)
        {
            var currentNode = myContext.Nodes[n];
            if (currentNode is IPauseNode)
                break;
            nonPauseNodes.Push(currentNode);
            myContext.Nodes.RemoveAt(n);
        }

        if (nonPauseNodes.Count == myContext.Nodes.Count)
        {
            parsingContext.LogError(reader, "Era esperada ao menos uma linha de diálogo.");
            return;
        }

        var block = parsingContext.BlockFactory.Create(myContext.Nodes);
        var node = new BalloonNode(BalloonType, block);
        parentParsingContext.AddNode(node);

        while (nonPauseNodes.TryPop(out var nonPauseNode))
            parentParsingContext.AddNode(nonPauseNode);
    }
}
