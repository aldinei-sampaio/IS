using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class BlockNodeTests
{
    [Fact]
    public void Initialization()
    {
        var block = A.Dummy<IBlock>();
        var sut = new BlockNode(block);
        sut.ChildBlock.Should().BeSameAs(block);
    }
}
