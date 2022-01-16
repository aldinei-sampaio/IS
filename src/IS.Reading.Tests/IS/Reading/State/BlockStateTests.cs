using IS.Reading.Navigation;

namespace IS.Reading.State;

public class BlockStateTests
{
    [Fact]
    public void Initialization()
    {
        var sut = new BlockState();
        sut.CurrentNode.Should().BeNull();
        sut.CurrentNodeIndex.Should().BeNull();
        sut.BackwardStack.Count.Should().Be(0);
    }

    [Fact]
    public void ReadWriteProperties()
    {
        var node = A.Dummy<INode>();

        var sut = new BlockState
        {
            CurrentNodeIndex = 157,
            CurrentNode = node
        };

        sut.CurrentNodeIndex.Should().Be(157);
        sut.CurrentNode.Should().BeSameAs(node);
    }
}
