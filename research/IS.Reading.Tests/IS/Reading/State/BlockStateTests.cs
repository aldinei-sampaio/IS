namespace IS.Reading.State;

public class BlockStateTests
{
    [Fact]
    public void GetCurrentIterationShouldCreateNewWhenCalledTheFirstTime()
    {
        var blockIterationState = A.Dummy<IBlockIterationState>();
        var blockStateFactory = A.Fake<IBlockStateFactory>(i => i.Strict());

        var sut = new BlockState(blockStateFactory);

        A.CallTo(() => blockStateFactory.CreateIterationState()).Returns(blockIterationState);

        sut.GetCurrentIteration().Should().BeSameAs(blockIterationState);

        A.CallTo(() => blockStateFactory.CreateIterationState()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void GetCurrentItemShouldReturnSameObjectWhenCalledRepeatedly()
    {
        var blockIterationState = A.Dummy<IBlockIterationState>();
        var blockStateFactory = A.Fake<IBlockStateFactory>(i => i.Strict());

        var sut = new BlockState(blockStateFactory);

        A.CallTo(() => blockStateFactory.CreateIterationState()).Returns(blockIterationState);

        sut.GetCurrentIteration().Should().BeSameAs(blockIterationState);
        sut.GetCurrentIteration().Should().BeSameAs(blockIterationState);
        sut.GetCurrentIteration().Should().BeSameAs(blockIterationState);
        sut.GetCurrentIteration().Should().BeSameAs(blockIterationState);
        sut.GetCurrentIteration().Should().BeSameAs(blockIterationState);
        
        A.CallTo(() => blockStateFactory.CreateIterationState()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void MoveToNextIterationShouldInitializeCurrentIteration()
    {
        var blockIterationState = A.Dummy<IBlockIterationState>();
        var blockStateFactory = A.Fake<IBlockStateFactory>(i => i.Strict());

        var sut = new BlockState(blockStateFactory);

        A.CallTo(() => blockStateFactory.CreateIterationState()).Returns(blockIterationState);

        sut.MoveToNextIteration();

        A.CallTo(() => blockStateFactory.CreateIterationState()).MustHaveHappenedOnceExactly();

        sut.GetCurrentIteration().Should().BeSameAs(blockIterationState);

        A.CallTo(() => blockStateFactory.CreateIterationState()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void MoveToNextAndMoveToPreviousIteration()
    {
        var state1 = A.Dummy<IBlockIterationState>();
        var state2 = A.Dummy<IBlockIterationState>();
        var state3 = A.Dummy<IBlockIterationState>();

        var blockStateFactory = A.Fake<IBlockStateFactory>(i => i.Strict());
        A.CallTo(() => blockStateFactory.CreateIterationState())
            .ReturnsNextFromSequence(state1, state2, state3);

        var sut = new BlockState(blockStateFactory);
        sut.GetCurrentIteration().Should().BeSameAs(state1);
        sut.MoveToNextIteration();
        sut.GetCurrentIteration().Should().BeSameAs(state2);
        sut.MoveToPreviousIteration().Should().BeTrue();
        sut.GetCurrentIteration().Should().BeSameAs(state1);
        sut.MoveToPreviousIteration().Should().BeFalse();
        sut.GetCurrentIteration().Should().BeSameAs(state3);
    }

    [Fact]
    public void MoveToPreviousShouldWorkWhenCalledMultipleTimes()
    {
        var blockIterationState = A.Dummy<IBlockIterationState>();
        var blockStateFactory = A.Fake<IBlockStateFactory>(i => i.Strict());
        A.CallTo(() => blockStateFactory.CreateIterationState()).Returns(blockIterationState);

        var sut = new BlockState(blockStateFactory);

        sut.MoveToPreviousIteration().Should().BeFalse();
        sut.MoveToPreviousIteration().Should().BeFalse();
        sut.MoveToPreviousIteration().Should().BeFalse();

        sut.GetCurrentIteration().Should().BeSameAs(blockIterationState);

        A.CallTo(() => blockStateFactory.CreateIterationState()).MustHaveHappenedOnceExactly();
    }
}
