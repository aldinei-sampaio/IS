using IS.Reading.Events;

namespace IS.Reading.Navigation;

public class StoryboardTests
{
    private readonly IBlock rootBlock;
    private readonly ISceneNavigator sceneNavigator;
    private readonly Storyboard sut;

    public StoryboardTests()
    {
        rootBlock = A.Dummy<IBlock>();
        sceneNavigator = A.Fake<ISceneNavigator>(i => i.Strict());
        sut = new Storyboard(rootBlock, sceneNavigator);
    }

    [Fact]
    public void Initialization()
    {
        sut.NavigationContext.Should().NotBeNull();
        sut.Events.Should().BeSameAs(sut.NavigationContext.Events);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task MoveAsync(bool forward, bool moveResult)
    {
        A.CallTo(() => sceneNavigator.MoveAsync(sut.NavigationContext, forward)).Returns(moveResult);

        var result = await sut.MoveAsync(forward);
        result.Should().Be(moveResult);

        A.CallTo(() => sceneNavigator.MoveAsync(sut.NavigationContext, forward)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Events()
    {
        var @event = new TestEvent();
        TestEvent received = null;

        sut.Events.Subscribe<TestEvent>(i =>
        {
            received = i;
            return Task.CompletedTask;
        });

        await sut.NavigationContext.Events.InvokeAsync(@event);

        received.Should().BeSameAs(@event);
    }

    private class TestEvent : IReadingEvent
    {
    }
}
