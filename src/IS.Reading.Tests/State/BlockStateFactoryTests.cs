namespace IS.Reading.State;

public class BlockStateFactoryTests
{
    private readonly IServiceProvider serviceProvider;
    private readonly BlockStateFactory sut;

    public BlockStateFactoryTests()
    {
        serviceProvider = A.Fake<IServiceProvider>(i => i.Strict());
        sut = new BlockStateFactory(serviceProvider);
    }

    [Fact]
    public void CreateIterationState()
    {
        A.CallTo(() => serviceProvider.GetService(typeof(IBlockIterationState)))
            .ReturnsLazily(i => A.Dummy<IBlockIterationState>());

        var f1 = sut.CreateIterationState();
        var f2 = sut.CreateIterationState();

        f1.Should().NotBeSameAs(f2);
        A.CallTo(() => serviceProvider.GetService(typeof(IBlockIterationState)))
            .MustHaveHappenedTwiceExactly();
    }

    [Fact]
    public void CreateState()
    {
        A.CallTo(() => serviceProvider.GetService(typeof(IBlockState)))
            .ReturnsLazily(i => A.Dummy<IBlockState>());

        var f1 = sut.CreateState();
        var f2 = sut.CreateState();

        f1.Should().NotBeSameAs(f2);

        A.CallTo(() => serviceProvider.GetService(typeof(IBlockState)))
            .MustHaveHappenedTwiceExactly();
    }

    [Fact]
    public void CreateStateDictionary()
    {
        A.CallTo(() => serviceProvider.GetService(typeof(IBlockStateDictionary)))
            .ReturnsLazily(i => A.Dummy<IBlockStateDictionary>());

        var f1 = sut.CreateStateDictionary();
        var f2 = sut.CreateStateDictionary();

        f1.Should().NotBeSameAs(f2);

        A.CallTo(() => serviceProvider.GetService(typeof(IBlockStateDictionary)))
            .MustHaveHappenedTwiceExactly();
    }
}
