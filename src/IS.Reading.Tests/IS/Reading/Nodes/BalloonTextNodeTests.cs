using IS.Reading.Choices;
using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class BalloonTextNodeTests
{
    private readonly IVariableDictionary variables;
    private readonly INavigationState state;
    private readonly INavigationContext context;
    private readonly ITextSource textSource;

    private const string balloonText = "alabama";

    public BalloonTextNodeTests()
    {
        variables = A.Dummy<IVariableDictionary>();
        state = A.Dummy<INavigationState>();
        context = A.Fake<INavigationContext>(i => i.Strict());
        A.CallTo(() => context.State).Returns(state);
        A.CallTo(() => context.Variables).Returns(variables);

        textSource = A.Dummy<ITextSource>();
        A.CallTo(() => textSource.Build(variables)).Returns(balloonText);
    }

    public static IEnumerable<object[]> GetBalloonTypes()
        => Enum.GetValues<BalloonType>().Select(i => new object[] { i });

    [Theory]
    [MemberData(nameof(GetBalloonTypes))]
    public void Initialization(BalloonType balloonType)
    {
        var sut = new BalloonTextNode(textSource, balloonType, null);
        sut.TextSource.Should().Be(textSource);
        sut.BalloonType.Should().Be(balloonType);
        sut.ChoiceBuilder.Should().BeNull();
    }

    [Fact]
    public void InitializeChoiceNode()
    {
        var choiceBuilder = A.Dummy<IChoiceBuilder>();
        var sut = new BalloonTextNode(textSource, BalloonType.Narration, choiceBuilder);
        sut.ChoiceBuilder.Should().BeSameAs(choiceBuilder);
    }

    [Theory]
    [InlineData(BalloonType.Narration, false)]
    [InlineData(BalloonType.Speech, true)]
    public async Task OnEnterAsyncShouldRaiseEvent(BalloonType balloonType, bool isProtagonist)
    {
        state.PersonName = "alpha";
        state.ProtagonistName = isProtagonist ? "alpha" : "beta";

        var sut = new BalloonTextNode(textSource, balloonType, null);
        var invoker = new TestInvoker(context);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeNull();

        invoker.ShouldContainSingle<IBalloonTextEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                Text = balloonText,
                BalloonType = balloonType,
                IsProtagonist = isProtagonist,
                Choice = (IChoice)null
            })
        );
    }

    [Fact]
    public async Task OnEnterAsyncWithChoice()
    {
        state.PersonName = "mu";
        state.ProtagonistName = "pi";

        var choice = A.Dummy<IChoice>();
        var choiceBuilder = A.Fake<IChoiceBuilder>(i => i.Strict());
        A.CallTo(() => choiceBuilder.Build(context)).Returns(choice);

        var sut = new BalloonTextNode(textSource, BalloonType.Speech, choiceBuilder);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IBalloonTextEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                Text = balloonText,
                BalloonType = BalloonType.Speech,
                IsProtagonist = false,
                Choice = choice
            })
        );

        A.CallTo(() => choiceBuilder.Build(context)).MustHaveHappenedOnceExactly();
    }
}