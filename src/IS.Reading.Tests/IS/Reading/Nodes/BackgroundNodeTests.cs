using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BackgroundNodeTests
{
    private readonly IBackgroundState initialState;
    private readonly IBackgroundState newState;
    private readonly ICondition when;
    private readonly INavigationContext context;
    private readonly BackgroundNode sut;

    public BackgroundNodeTests()
    {
        initialState = A.Dummy<IBackgroundState>();
        newState = A.Dummy<IBackgroundState>();
        when = A.Dummy<ICondition>();
        context = A.Dummy<INavigationContext>();
        context.State.Background = initialState;

        sut = new BackgroundNode(newState, when);
    }

    [Fact]
    public void ConstructorShouldInitializeStateAndWhen()
    {
        sut.State.Should().BeSameAs(newState);
        sut.When.Should().BeSameAs(when);
    }

    [Fact]
    public async Task EnterAsyncShouldRaiseEvent()
    {
        var invoker = new TestInvoker(context);
        await sut.EnterAsync(context);
        invoker.ShouldContainSingle<IBackgroundChangeEvent>(i => i.State.Should().BeSameAs(newState));
    }

    [Fact]
    public async Task EnterAsyncShouldChangeState()
    {
        await sut.EnterAsync(context);
        context.State.Background.Should().BeSameAs(newState);
    }

    [Fact]
    public async Task EnterAsyncShouldReturnAReversalNode()
    {
        var ret = await sut.EnterAsync(context);

        var retNode = ret.Should().BeOfType<BackgroundNode>().Which;
        retNode.State.Should().BeSameAs(initialState);
        retNode.When.Should().BeSameAs(when);
    }

    [Fact]
    public async Task ShouldNotRaiseEventIfThereIsNoChangeInBackground()
    {
        var invoker = new TestInvoker(context);

        context.State.Background = newState;
        await sut.EnterAsync(context);

        invoker.Count.Should().Be(0);
    }

    [Fact]
    public async Task ShouldReturnSelfIfThereIsNoChangeInBackground()
    {
        context.State.Background = newState;
        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(sut);    
    }

    [Fact]
    public async Task ShouldKeepStateIfThereIsNoChangeInBackground()
    {
        context.State.Background = newState;
        await sut.EnterAsync(context);
        context.State.Background.Should().BeSameAs(newState);
    }
}
