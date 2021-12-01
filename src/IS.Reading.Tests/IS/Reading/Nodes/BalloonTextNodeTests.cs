using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BalloonTextNodeTests
{
    public static IEnumerable<object[]> GetBalloonTypes()
        => Enum.GetValues<BalloonType>().Select(i => new object[] { i });

    [Theory]
    [MemberData(nameof(GetBalloonTypes))]
    public void Initialization(BalloonType balloonType)
    {
        var text = "Gibberish";
        var sut = new BalloonTextNode(text, balloonType);
        sut.Text.Should().Be(text);
        sut.BalloonType.Should().Be(balloonType);
    }

    [Theory]
    [InlineData(BalloonType.Narration, false, "Loren Ipsun")]
    [InlineData(BalloonType.Speech, true, "Shenanigans")]
    public async Task OnEnterAsyncShouldRaiseEvent(BalloonType balloonType, bool isProtagonist, string text)
    {
        var state = A.Dummy<INavigationState>();
        state.PersonName = "alpha";
        state.ProtagonistName = isProtagonist ? "alpha" : "beta";

        var context = A.Fake<INavigationContext>(i => i.Strict());
        A.CallTo(() => context.State).Returns(state);

        var sut = new BalloonTextNode(text, balloonType);
        var invoker = new TestInvoker(context);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(sut);

        var @event = invoker.Single<IBalloonTextEvent>();
        @event.Text.Should().Be(text);
        @event.BalloonType.Should().Be(balloonType);
        @event.IsProtagonist.Should().Be(isProtagonist);
    }
}
