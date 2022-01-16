namespace IS.Reading.Navigation;

public class BlockFactoryTests
{
    [Fact]
    public void Sequence()
    {
        var nodes = new List<INode>();

        var sut = new BlockFactory();
        for (var n = 0; n < 20; n++)
            sut.Create(nodes).Id.Should().Be(n);
    }

    [Fact]
    public void TwoArguments()
    {
        var n1 = A.Dummy<INode>();
        var n2 = A.Dummy<INode>();

        var sut = new BlockFactory();
        var result = sut.Create(n1, n2);

        result.Nodes.Should().BeEquivalentTo(new [] { n1, n2 });
    }
}
