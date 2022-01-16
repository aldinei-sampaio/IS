namespace IS.Reading.Navigation;

public class Block : IBlock
{
    public int Id { get; }

    public IReadOnlyList<INode> Nodes { get; }

    public Stack<INode> BackwardStack { get; } = new();

    public int? CurrentNodeIndex { get; set; }

    public INode? CurrentNode { get; set; }

    public Block(int id, IReadOnlyList<INode> nodes)
        => (Id, Nodes) = (id, nodes);
}
