namespace IS.Reading.State;

public class BlockStateDictionaryTests
{
    private readonly IBlockStateFactory blockStateFactory;
    private readonly BlockStateDictionary sut;

    public BlockStateDictionaryTests()
    {
        blockStateFactory = A.Fake<IBlockStateFactory>(i => i.Strict());
        A.CallTo(() => blockStateFactory.CreateState()).ReturnsLazily(i => A.Dummy<IBlockState>());
        sut = new(blockStateFactory);
    }

    [Fact]
    public void CreateWhenNotExists()
    {
        var state = sut[1982];
        state.Should().NotBeNull();
    }

    [Fact]
    public void ReuseWhenExists()
    {
        var s1 = sut[1234];
        var s2 = sut[1234];
        s1.Should().BeSameAs(s2);
    }

    [Fact]
    public void DifferentBlockIds()
    {
        var s1 = sut[7];
        var s2 = sut[8];
        s1.Should().NotBeSameAs(s2);
    }

    [Fact]
    public void InvalidBlockId()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => sut[-1]);
        ex.ParamName.Should().Be("blockId");
    }
}
