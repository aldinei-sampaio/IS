using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BackgroundChangeNodeTests
{
    [Fact]
    public void Initialization()
    {
        var newState = A.Dummy<IBackgroundState>();
        var when = A.Dummy<ICondition>();

        var sut = new BackgroundChangeNode(newState, when);

        sut.State.Should().BeSameAs(newState);
        sut.When.Should().BeSameAs(when);
    }

    [Fact]
    public async Task EnterAsync()
    {
        var oldState = A.Dummy<IBackgroundState>();
        var newState = A.Dummy<IBackgroundState>();
        var when = A.Dummy<ICondition>();

        var invoker = new TestInvoker();

        var context = A.Dummy<INavigationContext>();
        context.State.Background = oldState;
        A.CallTo(() => context.Events).Returns(invoker);

        var sut = new BackgroundChangeNode(newState, when);
        var ret = await sut.EnterAsync(context);

        invoker.Received.Should().HaveCount(1);
        invoker.Received[0].Should().BeOfType<BackgroundChangeEvent>()
            .Which.State.Should().BeSameAs(newState);

        context.State.Background.Should().BeSameAs(newState);

        var retNode = ret.Should().BeOfType<BackgroundChangeNode>().Which;
        retNode.State.Should().BeSameAs(oldState);
        retNode.When.Should().BeSameAs(when);
    }
}
