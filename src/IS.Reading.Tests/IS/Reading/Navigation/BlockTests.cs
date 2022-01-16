namespace IS.Reading.Navigation;

public class BlockTests
{
    [Fact]
    public void Initialization()
    {
        var nodeList = new List<INode>() { A.Dummy<INode>() };
        var sut = new Block(12345, nodeList);

        sut.Id.Should().Be(12345);
        sut.Nodes.Should().BeSameAs(nodeList);
    }
}
