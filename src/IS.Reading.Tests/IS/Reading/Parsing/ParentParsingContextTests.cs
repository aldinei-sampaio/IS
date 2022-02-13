using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public class ParentParsingContextTests
{
    [Fact]
    public void Initialization()
    {
        var sut = new ParentParsingContext();
        sut.Nodes.Should().BeEmpty();
    }

    [Fact]
    public void AddNode()
    {
        var node1 = A.Dummy<INode>();
        var node2 = A.Dummy<INode>();
        var node3 = A.Dummy<INode>();

        var sut = new ParentParsingContext();
        sut.AddNode(node1);
        sut.AddNode(node2);
        sut.AddNode(node3);

        sut.Nodes.Should().BeEquivalentTo(new[] { node1, node2, node3 });
    }
}