namespace IS.Reading.Navigation;

public class Block : IBlock
{
    public IReadOnlyList<INode> Nodes { get; }

    public Stack<INode> ForwardStack { get; } = new();

    public Stack<INode> BackwardStack { get; } = new();

    public int? CurrentNodeIndex { get; set; }

    public INode? CurrentNode { get; set; }

    public Block(IReadOnlyList<INode> nodes)
        => Nodes = nodes;

    public Block(INode node1, INode node2)
        => Nodes = new List<INode> { node1, node2 };
}
