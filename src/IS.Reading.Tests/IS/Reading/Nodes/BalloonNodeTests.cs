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
    public async Task OnEnterAsyncShouldRaiseEvent(BalloonType balloonType, bool isMainCharacter)
    {
        var tester = new Tester(balloonType, isMainCharacter);
        var sut = tester.BalloonNode;

        var ret = await sut.EnterAsync(tester.Context);
        ret.Should().BeNull();

        tester.Invoker.ShouldHadReceived<IBalloonOpenEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                BalloonType = balloonType,
                IsMainCharacter = isMainCharacter
            })
        );
    }

    [Theory]
    [InlineData(BalloonType.Narration, false)]
    [InlineData(BalloonType.Speech, true)]
    public async Task OnLeaveAsyncShouldRaiseEvent(BalloonType balloonType, bool isMainCharacter)
    {
        var tester = new Tester(balloonType, isMainCharacter);
        var sut = tester.BalloonNode;

        await sut.LeaveAsync(tester.Context);

        tester.Invoker.ShouldHadReceived<IBalloonCloseEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                BalloonType = balloonType,
                IsMainCharacter = isMainCharacter
            })
        );
    }

    private class Tester
    {
        public BalloonNode BalloonNode { get; }
        public TestInvoker Invoker { get; }
        public INavigationContext Context { get; }

        public Tester(BalloonType balloonType, bool isMainCharacter)
        {
            var state = A.Dummy<INavigationState>();
            state.PersonName = "alpha";
            state.MainCharacterName = isMainCharacter ? "alpha" : "beta";

            var context = A.Fake<INavigationContext>(i => i.Strict());
            A.CallTo(() => context.State).Returns(state);

            var childBlock = A.Dummy<IBlock>();

            BalloonNode = new BalloonNode(balloonType, childBlock);
            Context = context;
            Invoker = new TestInvoker(context);
        }
    }
}
