namespace IS.Reading.State;

public class BlockStateFactoryTests
{
    private readonly IServiceProvider serviceProvider;
    private readonly BlockStateFactory sut;

    public BlockStateFactoryTests()
    {
        serviceProvider = A.Fake<IServiceProvider>(i => i.Strict());
        A.CallTo(() => serviceProvider.GetService(typeof(IBlockState))).ReturnsLazily(i => A.Dummy<IBlockState>());
        sut = new BlockStateFactory(serviceProvider);
    }

    [Fact]
    public void Creation()
    {
        var f1 = sut.Create();
        var f2 = sut.Create();

        f1.Should().NotBeSameAs(f2);
        A.CallTo(() => serviceProvider.GetService(typeof(IBlockState))).MustHaveHappenedTwiceExactly();
    }
}
