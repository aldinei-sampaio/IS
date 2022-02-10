using IS.Reading.Conditions;
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
        var context = A.Dummy<INavigationContext>();
        var invoker = new TestInvoker(context);

        var duration = TimeSpan.FromMilliseconds(234567);

        var sut = new TimedPauseNode(duration, null);
        var ret = await sut.EnterAsync(context);
        ret.Should().BeNull();

        invoker.ShouldContainSingle<ITimedPauseEvent>(
            i => i.Should().BeEquivalentTo(new { Duration = duration })
        );
    }

    [Fact]
    public async Task EnterAsyncWithStateArgShouldDoNothing()
    {
        var context = A.Dummy<INavigationContext>();
        var invoker = new TestInvoker(context);

        var duration = TimeSpan.FromMilliseconds(234567);

        var sut = new TimedPauseNode(duration, null);
        await sut.EnterAsync(context, null);

        invoker.Count.Should().Be(0);
    }
}
