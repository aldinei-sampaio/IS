using IS.Reading.Events;
using IS.Reading.State;
using IS.Reading.Variables;

namespace IS.Reading.Navigation;

public class NavigationContextTests
{
    [Fact]
    public void Initialization()
    {
        var block = A.Dummy<IBlock>();
        var eventInvoker = A.Dummy<IEventInvoker>();
        var randomizer = A.Dummy<IRandomizer>();
        var navigationState = A.Dummy<INavigationState>();
        var variableDictionary = A.Dummy<IVariableDictionary>();

        var sut = new NavigationContext(block, eventInvoker, randomizer, navigationState, variableDictionary);

        sut.RootBlock.Should().BeSameAs(block);
        sut.Events.Should().BeSameAs(eventInvoker);
        sut.EnteredBlocks.Should().HaveCount(0);
        sut.State.Should().BeSameAs(navigationState);
        sut.CurrentBlock.Should().BeNull();
        sut.CurrentNode.Should().BeNull();
        sut.Variables.Should().BeSameAs(variableDictionary);
        sut.Randomizer.Should().BeSameAs(randomizer);
    }
}
