using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BackgroundDismissNodeTests
{
    private readonly IBackgroundState initialState;
    private readonly INavigationContext context;

    public BackgroundDismissNodeTests()
    {
        initialState = A.Dummy<IBackgroundState>();
        context = A.Dummy<INavigationContext>();
        context.State.Background = initialState;
    }

    [Fact]
    public async Task ConstructorAndEnterAsyncShouldInitializeBackgroundChangeNodeProperty()
    {
        var reversedNode = A.Fake<INode>();
        var node = A.Fake<INode>();
        A.CallTo(() => node.EnterAsync(context)).Returns(reversedNode);

        var sut = new BackgroundDismissNode(node);
        sut.BackgroundChangeNode.Should().BeSameAs(node);

        var sut2 = (BackgroundDismissNode)await sut.EnterAsync(context);
        sut2.BackgroundChangeNode.Should().BeSameAs(reversedNode);
    }

    [Fact]
    public async Task EnterAsyncShouldUpdateState()
    {
        var sut = new BackgroundDismissNode();

        for (var n = 1; n <= 3; n++)
        {
            sut = (BackgroundDismissNode)await sut.EnterAsync(context);
            context.State.Background.Should().BeSameAs(BackgroundState.Empty);

            sut = (BackgroundDismissNode)await sut.EnterAsync(context);
            context.State.Background.Should().BeSameAs(initialState);
        }
    }

    [Fact]
    public async Task EnterAsyncShouldReturnUpdatedBackgroundChangeNode()
    {
        var sut = new BackgroundDismissNode();

        for (var n = 1; n <= 3; n++)
        {
            sut = (BackgroundDismissNode)await sut.EnterAsync(context);
            ((BackgroundChangeNode)sut.BackgroundChangeNode).State.Should().BeSameAs(initialState);

            sut = (BackgroundDismissNode)await sut.EnterAsync(context);
            ((BackgroundChangeNode)sut.BackgroundChangeNode).State.Should().BeSameAs(BackgroundState.Empty);
        }
    }

    [Fact]
    public async Task EnterAsyncShouldRaiseEvent()
    {
        var invoker = new TestInvoker(context);
        var sut = new BackgroundDismissNode();

        for (var n = 1; n <= 3; n++)
        {
            sut = (BackgroundDismissNode)(await sut.EnterAsync(context));
            invoker.Single<IBackgroundChangeEvent>().State.Should().Be(BackgroundState.Empty);
            invoker.Clear();

            sut = (BackgroundDismissNode)(await sut.EnterAsync(context));
            invoker.Single<IBackgroundChangeEvent>().State.Should().Be(initialState);
            invoker.Clear();
        }
    }

    [Fact]
    public async Task ShouldReturnSelfIfThereIsNoBackgroundChange()
    {
        var sut = new BackgroundDismissNode();

        context.State.Background = BackgroundState.Empty;
        var ret = await sut.EnterAsync(context);

        ret.Should().BeSameAs(sut);
    }

    [Fact]
    public async Task ShouldNotRaiseEventIfThereIsNoBackgroundChange()
    {
        var invoker = new TestInvoker(context);
        var sut = new BackgroundDismissNode();

        context.State.Background = BackgroundState.Empty;
        await sut.EnterAsync(context);

        invoker.Count.Should().Be(0);
    }
}
