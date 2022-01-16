namespace IS.Reading.Navigation;

public class BlockTests
{
    [Fact]
    public void Initialization()
    {
        var nodeList = new List<INode> { A.Dummy<INode>() };
        var sut = new Block(nodeList);

        sut.CurrentNode.Should().BeNull();
        sut.CurrentNodeIndex.Should().BeNull();
        sut.Nodes.Should().BeSameAs(nodeList);
        sut.BackwardStack.Count.Should().Be(0);
    }

    [Fact]
    public void ReadWriteProperties()
    {
        var nodeList = new List<INode> { A.Dummy<INode>() };
        var sut = new Block(nodeList);

        sut.CurrentNodeIndex = 157;
        sut.CurrentNode = nodeList[0];

        sut.CurrentNodeIndex.Should().Be(157);
        sut.CurrentNode.Should().BeSameAs(nodeList[0]);
    }
}
