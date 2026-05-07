using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BackgroundNodeTests
{
    private readonly IBackgroundState initialState;
    private readonly IBackgroundState newState;
    private readonly INavigationContext context;
    private readonly BackgroundNode sut;

    public BackgroundNodeTests()
    {
        initialState = A.Dummy<IBackgroundState>();
        newState = A.Dummy<IBackgroundState>();
        context = A.Dummy<INavigationContext>();
        context.State.Background = initialState;

        sut = new BackgroundNode(newState);
    }

    [Fact]
    public void ConstructorShouldInitializeProperties()
    {
        sut.State.Should().BeSameAs(newState);
        sut.Animation.Should().Be(BackgroundAnimation.None);
        sut.FlashColor.Should().BeNull();
    }

    [Fact]
    public void ConstructorShouldInitializeAnimationProperties()
    {
        var node = new BackgroundNode(newState, BackgroundAnimation.Flash, "white");
        node.Animation.Should().Be(BackgroundAnimation.Flash);
        node.FlashColor.Should().Be("white");
    }

    [Fact]
    public async Task EnterAsyncShouldRaiseEvent()
    {
        var invoker = new TestInvoker(context);
        await sut.EnterAsync(context);
        invoker.ShouldHadReceived<IBackgroundChangeEvent>(i =>
        {
            i.State.Should().BeSameAs(newState);
            i.Animation.Should().Be(BackgroundAnimation.None);
            i.FlashColor.Should().BeNull();
        });
    }

    [Theory]
    [InlineData(BackgroundAnimation.FadeIn,   null)]
    [InlineData(BackgroundAnimation.Zoom,     null)]
    [InlineData(BackgroundAnimation.Dissolve, null)]
    [InlineData(BackgroundAnimation.Flash,    null)]
    [InlineData(BackgroundAnimation.Flash,    "white")]
    public async Task EnterAsyncShouldForwardAnimationToEvent(BackgroundAnimation animation, string? flashColor)
    {
        var node = new BackgroundNode(newState, animation, flashColor);
        var invoker = new TestInvoker(context);
        await node.EnterAsync(context);
        invoker.ShouldHadReceived<IBackgroundChangeEvent>(i =>
        {
            i.Animation.Should().Be(animation);
            i.FlashColor.Should().Be(flashColor);
        });
    }

    [Fact]
    public async Task BackwardShouldRestoreStateWithNoAnimation()
    {
        var node = new BackgroundNode(newState, BackgroundAnimation.FadeIn, null);
        await node.EnterAsync(context);

        var invoker = new TestInvoker(context);
        await node.EnterAsync(context, initialState);
        invoker.ShouldHadReceived<IBackgroundChangeEvent>(i =>
        {
            i.State.Should().BeSameAs(initialState);
            i.Animation.Should().Be(BackgroundAnimation.None);
            i.FlashColor.Should().BeNull();
        });
    }

    [Fact]
    public async Task EnterAsyncShouldChangeState()
    {
        await sut.EnterAsync(context);
        context.State.Background.Should().BeSameAs(newState);
    }

    [Fact]
    public async Task EnterAsyncShouldReturnPreviousState()
    {
        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(initialState);
    }

    [Fact]
    public async Task ShouldNotRaiseEventIfThereIsNoChangeInBackground()
    {
        var invoker = new TestInvoker(context);

        context.State.Background = newState;
        await sut.EnterAsync(context);

        invoker.HadReceivedEvent.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldReturnPreviousStateEvenIfThereIsNoChangeInBackground()
    {
        context.State.Background = newState;
        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(newState);
    }

    [Fact]
    public async Task ShouldKeepStateIfThereIsNoChangeInBackground()
    {
        context.State.Background = newState;
        await sut.EnterAsync(context);
        context.State.Background.Should().BeSameAs(newState);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Abc")]
    [InlineData(123)]
    public async Task ShouldDoNothingWhenStateArgIsNotBackgroundState(object? stateArg)
    {
        var invoker = new TestInvoker(context);
        await sut.EnterAsync(context, stateArg);
        invoker.HadReceivedEvent.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldRaiseEventWhenStateArgIsBackgroundState()
    {
        var invoker = new TestInvoker(context);
        await sut.EnterAsync(context, newState);
        invoker.ShouldHadReceived<IBackgroundChangeEvent>(i => i.State.Should().BeSameAs(newState));
    }
}
