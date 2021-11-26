using IS.Reading.Events;

namespace IS.Reading.Navigation;

public class NavigationContextTests
{
    [Fact]
    public void Initialization()
    {
        var block = A.Dummy<IBlock>();
        var eventInvoker = A.Dummy<IEventInvoker>();

        var sut = new NavigationContext(block, eventInvoker);

        sut.RootBlock.Should().BeSameAs(block);
        sut.Events.Should().BeSameAs(eventInvoker);
        sut.EnteredBlocks.Should().HaveCount(0);
        sut.State.Should().NotBeNull();
        sut.State.Background.Should().NotBeNull();
        sut.State.Background.Name.Should().Be(string.Empty);
        sut.State.Background.Position.Should().Be(State.BackgroundPosition.Undefined);
        sut.State.Background.Type.Should().Be(State.BackgroundType.Undefined);
        sut.CurrentBlock.Should().BeNull();
        sut.CurrentNode.Should().BeNull();
        sut.Variables.Should().NotBeNull();
    }
}
