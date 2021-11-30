using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BalloonNodeTests
{
    public static IEnumerable<object[]> GetBalloonTypes()
        => Enum.GetValues<BalloonType>().Select(i => new object[] { i });

    [Theory]
    [MemberData(nameof(GetBalloonTypes))]
    public void Initialization(BalloonType balloonType)
    {
        var childBlock = A.Dummy<IBlock>();
        var sut = new BalloonNode(balloonType, childBlock);
        sut.BallonType.Should().Be(balloonType);
        sut.ChildBlock.Should().Be(childBlock);
    }

    [Theory]
    [InlineData(BalloonType.Narration, false)]
    [InlineData(BalloonType.Speech, true)]
    public async Task OnEnterAsyncShouldRaiseEvent(BalloonType balloonType, bool isProtagonist)
    {
        var tester = new Tester(balloonType, isProtagonist);
        var sut = tester.BalloonNode;

        var ret = await sut.EnterAsync(tester.Context);
        ret.Should().BeSameAs(sut);

        var @event = tester.Invoker.Single<IBalloonOpenEvent>();
        @event.BallonType.Should().Be(balloonType);
        @event.IsProtagonist.Should().Be(isProtagonist);
    }

    [Theory]
    [InlineData(BalloonType.Narration, false)]
    [InlineData(BalloonType.Speech, true)]
    public async Task OnLeaveAsyncShouldRaiseEvent(BalloonType balloonType, bool isProtagonist)
    {
        var tester = new Tester(balloonType, isProtagonist);
        var sut = tester.BalloonNode;

        var ret = await sut.LeaveAsync(tester.Context);
        ret.Should().BeSameAs(sut);

        var @event = tester.Invoker.Single<IBalloonCloseEvent>();
        @event.BallonType.Should().Be(balloonType);
        @event.IsProtagonist.Should().Be(isProtagonist);
    }

    private class Tester
    {
        public BalloonNode BalloonNode { get; }
        public TestInvoker Invoker { get; }
        public INavigationContext Context { get; }

        public Tester(BalloonType balloonType, bool isProtagonist)
        {
            var state = A.Dummy<INavigationState>();
            state.PersonName = "alpha";
            state.Protagonist = isProtagonist ? "alpha" : "beta";

            var context = A.Fake<INavigationContext>(i => i.Strict());
            A.CallTo(() => context.State).Returns(state);

            var childBlock = A.Dummy<IBlock>();

            BalloonNode = new BalloonNode(balloonType, childBlock);
            Context = context;
            Invoker = new TestInvoker(context);
        }
    }
}
