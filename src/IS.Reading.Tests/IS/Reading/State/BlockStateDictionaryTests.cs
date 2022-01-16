namespace IS.Reading.State;

public class BlockStateDictionaryTests
{
    private readonly IBlockStateFactory blockStateFactory;
    private readonly BlockStateDictionary sut;

    public BlockStateDictionaryTests()
    {
        blockStateFactory = A.Fake<IBlockStateFactory>(i => i.Strict());
        A.CallTo(() => blockStateFactory.Create()).ReturnsLazily(i => A.Dummy<IBlockState>());
        sut = new(blockStateFactory);
    }

    [Fact]
    public void CreateWhenNotExists()
    {
        var state = sut[1982, 0];
        state.Should().NotBeNull();
    }

    [Fact]
    public void ReuseWhenExists()
    {
        var s1 = sut[1234, 1];
        var s2 = sut[1234, 1];
        s1.Should().BeSameAs(s2);
    }

    [Fact]
    public void DifferentBlockIds()
    {
        var s1 = sut[7, 0];
        var s2 = sut[8, 0];
        s1.Should().NotBeSameAs(s2);
    }

    [Fact]
    public void DifferentIterations()
    {
        var s1 = sut[5, 0];
        var s2 = sut[5, 1];
        s1.Should().NotBeSameAs(s2);
    }

    [Fact]
    public void RememberChanges()
    {
        sut[7, 0].CurrentNodeIndex = 15;
        sut[7, 0].CurrentNodeIndex.Should().Be(15);
    }

    [Fact]
    public void InvalidBlockId()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => sut[-1, 0]);
        ex.ParamName.Should().Be("blockId");
    }

    [Fact]
    public void InvalidIteration()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => sut[18, -4]);
        ex.ParamName.Should().Be("iteration");
    }
}
