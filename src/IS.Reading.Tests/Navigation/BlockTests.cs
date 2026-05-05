using IS.Reading.Conditions;

namespace IS.Reading.Navigation;

public class BlockTests
{
    [Fact]
    public void InitializationWithoutWhile()
    {
        var nodeList = new List<INode>() { A.Dummy<INode>() };
        var sut = new Block(12345, nodeList, null);

        sut.Id.Should().Be(12345);
        sut.Nodes.Should().BeSameAs(nodeList);
        sut.While.Should().BeNull();
    }

    [Fact]
    public void InitializationWithWhile()
    {
        var @while = A.Dummy<ICondition>();
        var nodeList = new List<INode>() { A.Dummy<INode>() };
        var sut = new Block(12345, nodeList, @while);

        sut.Id.Should().Be(12345);
        sut.Nodes.Should().BeSameAs(nodeList);
        sut.While.Should().BeSameAs(@while);
    }
}
