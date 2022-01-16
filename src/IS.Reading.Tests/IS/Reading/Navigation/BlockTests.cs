namespace IS.Reading.Navigation;

public class BlockTests
{
    private readonly List<INode> nodeList;
    private readonly Block sut;

    public BlockTests()
    {
        nodeList = new() { A.Dummy<INode>() };
        sut = new(12345, nodeList);
    }

    [Fact]
    public void Initialization()
    {
        sut.Id.Should().Be(12345);
        sut.CurrentNode.Should().BeNull();
        sut.CurrentNodeIndex.Should().BeNull();
        sut.Nodes.Should().BeSameAs(nodeList);
        sut.BackwardStack.Count.Should().Be(0);
    }

    [Fact]
    public void ReadWriteProperties()
    {
        sut.CurrentNodeIndex = 157;
        sut.CurrentNode = nodeList[0];

        sut.CurrentNodeIndex.Should().Be(157);
        sut.CurrentNode.Should().BeSameAs(nodeList[0]);
    }
}
