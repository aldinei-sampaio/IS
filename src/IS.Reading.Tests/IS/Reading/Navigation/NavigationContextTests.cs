using IS.Reading.Events;
using IS.Reading.State;
using IS.Reading.Variables;

namespace IS.Reading.Navigation;

public class NavigationContextTests
{
    private readonly IBlock rootBlock;
    private readonly IBlockState rootBlockState;
    private readonly IEventInvoker eventInvoker;
    private readonly IRandomizer randomizer;
    private readonly INavigationState navigationState;
    private readonly IVariableDictionary variableDictionary;
    private readonly NavigationContext sut;

    public NavigationContextTests()
    {
        rootBlock = A.Dummy<IBlock>();
        rootBlockState = A.Dummy<IBlockState>();
        eventInvoker = A.Dummy<IEventInvoker>();
        randomizer = A.Dummy<IRandomizer>();
        navigationState = A.Dummy<INavigationState>();
        variableDictionary = A.Dummy<IVariableDictionary>();

        sut = new NavigationContext(
            rootBlock,
            rootBlockState,
            eventInvoker,
            randomizer,
            navigationState,
            variableDictionary
        );
    }

    [Fact]
    public void Initialization()
    {
        sut.RootBlock.Should().BeSameAs(rootBlock);
        sut.RootBlockState.Should().BeSameAs(rootBlockState);
        sut.Events.Should().BeSameAs(eventInvoker);
        sut.EnteredBlocks.Should().HaveCount(0);
        sut.State.Should().BeSameAs(navigationState);
        sut.CurrentBlock.Should().BeNull();
        sut.CurrentBlockState.Should().BeNull();
        sut.CurrentNode.Should().BeNull();
        sut.Variables.Should().BeSameAs(variableDictionary);
        sut.Randomizer.Should().BeSameAs(randomizer);
    }

    [Fact]
    public void WriteableProperties()
    {
        var block = A.Dummy<IBlock>();
        var node = A.Dummy<INode>();
        var blockState = A.Dummy<IBlockState>();

        sut.CurrentBlock = block;
        sut.CurrentBlockState = blockState;
        sut.CurrentNode = node;

        sut.ShouldSatisfy(i =>
        {
            i.CurrentBlock.Should().BeSameAs(block);
            i.CurrentBlockState.Should().BeSameAs(blockState);
            i.CurrentNode.Should().BeSameAs(node);
        });
    }
}
