using IS.Reading.Navigation;
using IS.Reading.Nodes;

namespace IS.Reading.Parsing.NodeParsers;

public abstract class DialogNodeParserBase : IAggregateNodeParser
{
    public IElementParser ElementParser { get; }

    public IElementParserSettings Settings { get; }

    public DialogNodeParserBase(IElementParser elementParser, params INodeParser[] nodes)
    {
        ElementParser = elementParser;
        Settings = new ElementParserSettings.AggregatedCurrent(nodes);
    }

    public bool IsArgumentRequired => true;

    public abstract string Name { get; }

    public abstract BalloonType BalloonType { get; }

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new BalloonParsingContext(BalloonType);
        await ElementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (!parsingContext.IsSuccess)
            return;

        var extraNonPauseNodes = ExtractNonPauseNodesAfterLastPauseNode(myContext.Nodes);

        var block = parsingContext.BlockFactory.Create(myContext.Nodes);
        var node = new BalloonNode(BalloonType, block);
        parentParsingContext.AddNode(node);

        while (extraNonPauseNodes.TryPop(out var nonPauseNode))
            parentParsingContext.AddNode(nonPauseNode);
    }

    private static Stack<INode> ExtractNonPauseNodesAfterLastPauseNode(List<INode> nodes)
    {
        var nonPauseNodes = new Stack<INode>();
        for (var n = nodes.Count - 1; n >= 0; n--)
        {
            var currentNode = nodes[n];
            if (currentNode is IPauseNode)
                break;
            nonPauseNodes.Push(currentNode);
            nodes.RemoveAt(n);
        }
        return nonPauseNodes;
    }
}
