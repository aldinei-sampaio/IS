using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public class FakeBlockFactory : IBlockFactory
{
    public IBlock Create(IReadOnlyList<INode> nodes)
    {
        var block = A.Dummy<IBlock>();
        A.CallTo(() => block.Nodes).Returns(nodes);
        return block;
    }

    public IBlock Create(INode node1, INode node2)
        => Create(new List<INode> { node1, node2 });
}
