namespace IS.Reading.State;

public class BlockStateFactoryTests
{
    private readonly IServiceProvider serviceProvider;
    private readonly BlockStateFactory sut;

    public BlockStateFactoryTests()
    {
        serviceProvider = A.Fake<IServiceProvider>(i => i.Strict());
        A.CallTo(() => serviceProvider.GetService(typeof(IBlockIterationState))).ReturnsLazily(i => A.Dummy<IBlockIterationState>());
        sut = new BlockStateFactory(serviceProvider);
    }

    [Fact]
    public void Creation()
    {
        var f1 = sut.CreateIterationState();
        var f2 = sut.CreateIterationState();

        f1.Should().NotBeSameAs(f2);
        A.CallTo(() => serviceProvider.GetService(typeof(IBlockIterationState))).MustHaveHappenedTwiceExactly();
    }
}
