using IS.Reading.Navigation;

namespace IS.Reading.State;

public class BlockStateTests
{
    private readonly IBlockStateFactory blockStateFactory;
    private readonly BlockIterationState sut;

    public BlockStateTests()
    {
        blockStateFactory = A.Fake<IBlockStateFactory>(i => i.Strict());
        sut = new BlockIterationState(blockStateFactory);
    }

    [Fact]
    public void Initialization()
    {
        sut.CurrentNode.Should().BeNull();
        sut.CurrentNodeIndex.Should().BeNull();
        sut.BackwardStack.Count.Should().Be(0);
    }

    [Fact]
    public void ReadWriteProperties()
    {
        var node = A.Dummy<INode>();

        sut.CurrentNodeIndex = 157;
        sut.CurrentNode = node;

        sut.CurrentNodeIndex.Should().Be(157);
        sut.CurrentNode.Should().BeSameAs(node);
    }

    [Fact]
    public void InitializeChildren()
    {
        var dic = A.Dummy<IBlockStateDictionary>();
        A.CallTo(() => blockStateFactory.CreateStateDictionary()).Returns(dic);

        sut.Children.Should().BeSameAs(dic);

        A.CallTo(() => blockStateFactory.CreateStateDictionary()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void DoubleCallShouldNotReinitializeChildren()
    {
        var dic = A.Dummy<IBlockStateDictionary>();
        A.CallTo(() => blockStateFactory.CreateStateDictionary()).Returns(dic);

        sut.Children.Should().BeSameAs(dic);
        sut.Children.Should().BeSameAs(dic);

        A.CallTo(() => blockStateFactory.CreateStateDictionary()).MustHaveHappenedOnceExactly();
    }
}
