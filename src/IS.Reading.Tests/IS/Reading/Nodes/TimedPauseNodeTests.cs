using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class TimedPauseNodeTests
{
    [Fact]
    public void Initialization()
    {
        var duration = TimeSpan.FromMilliseconds(123456);
        var sut = new TimedPauseNode(duration);
        sut.Duration.Should().Be(duration);
    }

    [Fact]
    public async Task EnterAsync()
    {
        var context = A.Dummy<INavigationContext>();
        var invoker = new TestInvoker(context);

        var duration = TimeSpan.FromMilliseconds(234567);

        var sut = new TimedPauseNode(duration);
        var ret = await sut.EnterAsync(context);
        ret.Should().BeNull();

        invoker.ShouldHadReceived<ITimedPauseEvent>(
            i => i.Should().BeEquivalentTo(new { Duration = duration })
        );
    }

    [Fact]
    public async Task EnterAsyncWithStateArgShouldDoNothing()
    {
        var context = A.Dummy<INavigationContext>();
        var invoker = new TestInvoker(context);

        var duration = TimeSpan.FromMilliseconds(234567);

        var sut = new TimedPauseNode(duration);
        await sut.EnterAsync(context, null);

        invoker.HadReceivedEvent.Should().BeFalse();
    }
}
