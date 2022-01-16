namespace IS.Reading.Navigation;

public class Block : IBlock
{
    public int Id { get; }

    public IReadOnlyList<INode> Nodes { get; }

    public Block(int id, IReadOnlyList<INode> nodes)
        => (Id, Nodes) = (id, nodes);
}
