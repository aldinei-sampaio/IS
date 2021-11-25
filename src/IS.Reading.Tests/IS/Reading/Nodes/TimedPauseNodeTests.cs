using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class TimedPauseNodeTests
{
    [Fact]
    public void Initialization()
    {
        var when = A.Dummy<ICondition>();
        var duration = TimeSpan.FromMilliseconds(123456);

        var sut = new TimedPauseNode(duration, when);
        sut.When.Should().BeSameAs(when);
        sut.Duration.Should().Be(duration);
    }

    [Fact]
    public async Task EnterAsync()
    {
        var invoker = new TestInvoker();

        var context = A.Dummy<INavigationContext>();
        A.CallTo(() => context.Events).Returns(invoker);

        var duration = TimeSpan.FromMilliseconds(234567);

        var sut = new TimedPauseNode(duration, null);
        var ret = await sut.EnterAsync(context);

        invoker.Received.Should().HaveCount(1);
        invoker.Received[0].Should().BeOfType<TimedPauseEvent>()
            .Which.Duration.Should().Be(duration);

        ret.Should().BeSameAs(ret);
    }
}
