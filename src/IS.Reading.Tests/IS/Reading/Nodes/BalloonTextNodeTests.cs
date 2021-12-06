using IS.Reading.Choices;
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
        var sut = new BalloonTextNode(text, balloonType, null);
        sut.Text.Should().Be(text);
        sut.BalloonType.Should().Be(balloonType);
        sut.ChoiceNode.Should().BeNull();
    }

    [Fact]
    public void InitializeChoiceNode()
    {
        var choiceNode = A.Dummy<IChoiceNode>();
        var sut = new BalloonTextNode("abc", BalloonType.Narration, choiceNode);
        sut.ChoiceNode.Should().BeSameAs(choiceNode);
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

        var sut = new BalloonTextNode(text, balloonType, null);
        var invoker = new TestInvoker(context);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(sut);

        var @event = invoker.Single<IBalloonTextEvent>();
        @event.Text.Should().Be(text);
        @event.BalloonType.Should().Be(balloonType);
        @event.IsProtagonist.Should().Be(isProtagonist);
        @event.Choice.Should().BeNull();
    }
}
