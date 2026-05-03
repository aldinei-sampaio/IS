using IS.Reading.Conditions;

namespace IS.Reading.Navigation;

public class BlockFactory : IBlockFactory
{
    private int nextIndex = 0;

    public IBlock Create(IReadOnlyList<INode> nodes)
        => Create(nodes, null);

    public IBlock Create(IReadOnlyList<INode> nodes, ICondition? @while)
        => new Block(nextIndex++, nodes, @while);

    public IBlock Create(INode node1, INode node2)
        => Create(new List<INode> { node1, node2 }, null);
}
