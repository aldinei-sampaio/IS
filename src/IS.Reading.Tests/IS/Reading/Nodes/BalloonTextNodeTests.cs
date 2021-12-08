using IS.Reading.Choices;
using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BalloonTextNodeTests
{
    private readonly IVariableDictionary variables;
    private readonly INavigationState state;
    private readonly INavigationContext context;

    public BalloonTextNodeTests()
    {
        variables = A.Dummy<IVariableDictionary>();
        state = A.Dummy<INavigationState>();
        context = A.Fake<INavigationContext>(i => i.Strict());
        A.CallTo(() => context.State).Returns(state);
        A.CallTo(() => context.Variables).Returns(variables);
    }

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

    [Fact]
    public async Task OnEnterAsyncWithChoice()
    {
        state.PersonName = "mu";
        state.ProtagonistName = "pi";

        var choiceNode = new TestChoiceNode
        {
            TimeLimit = TimeSpan.FromSeconds(3),
            Default = "a",
            Options = new()
            {
                new TestChoiceOptionNode { Key = "a", Text = "Opção1" },
                new TestChoiceOptionNode { Key = "b", Text = "Opção2", ImageName = "abc", HelpText = "help" }
            }
        };

        var sut = new BalloonTextNode("...", BalloonType.Speech, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        var @event = invoker.Single<IBalloonTextEvent>();
        @event.Text.Should().Be("...");
        @event.BalloonType.Should().Be(BalloonType.Speech);
        @event.IsProtagonist.Should().BeFalse();
        @event.Choice.Should().NotBeNull();
        @event.Choice.TimeLimit.Should().Be(TimeSpan.FromSeconds(3));
        @event.Choice.Default.Should().Be("a");

        var eventOptions = @event.Choice.Options.ToList();
        eventOptions.Should().HaveCount(2);
        eventOptions[0].ShouldBe("a", "Opção1", true, null, null);
        eventOptions[1].ShouldBe("b", "Opção2", true, "abc", "help");
    }

    [Fact]
    public async Task OnEnterAsyncShouldEvaluateOptionEnabledCondition()
    {
        var when1 = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => when1.Evaluate(variables)).Returns(true);
        var when2 = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => when2.Evaluate(variables)).Returns(false);
        var when3 = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => when3.Evaluate(variables)).Returns(false);

        var choiceNode = new TestChoiceNode
        {
            Options = new()
            {
                new TestChoiceOptionNode { Key = "a", Text = "Opção1", DisabledText = "Inativa", EnabledWhen = when1 },
                new TestChoiceOptionNode { Key = "b", Text = "Opção2", DisabledText = "Não ativa", EnabledWhen = when2 },
                new TestChoiceOptionNode { Key = "c", Text = "Opção3", EnabledWhen = when3 }
            }
        };

        var sut = new BalloonTextNode("...", BalloonType.Speech, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        var @event = invoker.Single<IBalloonTextEvent>();
        var eventOptions = @event.Choice.Options.ToList();
        eventOptions.Should().HaveCount(3);
        eventOptions[0].ShouldBe("a", "Opção1", true, null, null);
        eventOptions[1].ShouldBe("b", "Não ativa", false, null, null);
        eventOptions[2].ShouldBe("c", "Opção3", false, null, null);

        A.CallTo(() => when1.Evaluate(variables)).MustHaveHappenedOnceExactly();
        A.CallTo(() => when2.Evaluate(variables)).MustHaveHappenedOnceExactly();
        A.CallTo(() => when3.Evaluate(variables)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task OnEnterAsyncShouldEvaluateOptionVisibleCondition()
    {
        var visible1 = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => visible1.Evaluate(variables)).Returns(false);
        var visible2 = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => visible2.Evaluate(variables)).Returns(true);

        var choiceNode = new TestChoiceNode
        {
            Options = new()
            {
                new TestChoiceOptionNode { Key = "a", Text = "Opção1", VisibleWhen = visible1 },
                new TestChoiceOptionNode { Key = "b", Text = "Opção2", VisibleWhen = visible2 }
            }
        };

        var sut = new BalloonTextNode("...", BalloonType.Speech, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        var @event = invoker.Single<IBalloonTextEvent>();
        var eventOptions = @event.Choice.Options.ToList();
        eventOptions.Should().HaveCount(1);
        eventOptions[0].ShouldBe("b", "Opção2", true, null, null);

        A.CallTo(() => visible1.Evaluate(variables)).MustHaveHappenedOnceExactly();
        A.CallTo(() => visible2.Evaluate(variables)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ChoiceMustBeNullWhenThereAreNoOptions()
    {
        var when = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => when.Evaluate(variables)).Returns(false);

        var choiceNode = new TestChoiceNode { Options = new() };

        var sut = new BalloonTextNode("...", BalloonType.Speech, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        var @event = invoker.Single<IBalloonTextEvent>();
        @event.Choice.Should().BeNull();
    }

    [Fact]
    public async Task ChoiceMustBeNullWhenAllOptionsAreDisabled()
    {
        var when = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => when.Evaluate(variables)).Returns(false);

        var choiceNode = new TestChoiceNode
        {
            Options = new()
            {
                new TestChoiceOptionNode { Key = "a", Text = "Opção1", EnabledWhen = when },
                new TestChoiceOptionNode { Key = "b", Text = "Opção2", EnabledWhen = when }
            }
        };

        var sut = new BalloonTextNode("...", BalloonType.Speech, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        var @event = invoker.Single<IBalloonTextEvent>();
        @event.Choice.Should().BeNull();
    }

    [Fact]
    public async Task ChoiceMustBeNullWhenAllOptionsAreInvisible()
    {
        var when = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => when.Evaluate(variables)).Returns(false);

        var choiceNode = new TestChoiceNode
        {
            Options = new()
            {
                new TestChoiceOptionNode { Key = "a", Text = "Opção1", VisibleWhen = when },
                new TestChoiceOptionNode { Key = "b", Text = "Opção2", VisibleWhen = when }
            }
        };

        var sut = new BalloonTextNode("...", BalloonType.Speech, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        var @event = invoker.Single<IBalloonTextEvent>();
        @event.Choice.Should().BeNull();
    }

    [Fact]
    public async Task RandomOrder()
    {
        var choiceNode = new TestChoiceNode
        {
            RandomOrder = true,
            Options = new()
            {
                new TestChoiceOptionNode { Key = "a", Text = "Opção1"},
                new TestChoiceOptionNode { Key = "b", Text = "Opção2"}
            }
        };

        var shuffled = new List<IChoiceOption>
        {
            A.Fake<IChoiceOption>(i => i.ConfigureFake(i => {
                A.CallTo(() => i.IsEnabled).Returns(true);
            }))
        };

        var randomizer = A.Fake<IRandomizer>(i => i.Strict());
        A.CallTo(() => randomizer.Shuffle(A<List<IChoiceOption>>.Ignored)).Returns(shuffled);
        A.CallTo(() => context.Randomizer).Returns(randomizer);

        var sut = new BalloonTextNode("...", BalloonType.Speech, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        var @event = invoker.Single<IBalloonTextEvent>();
        @event.Choice.Options.Should().BeSameAs(shuffled);
    }

    private class TestChoiceNode : IChoiceNode
    {
        public TimeSpan? TimeLimit { get; set; }

        public string Default { get; set; }

        public List<IChoiceOptionNode> Options { get; set;  }

        public bool RandomOrder { get; set; }

        IEnumerable<IChoiceOptionNode> IChoiceNode.Options => Options;
    }

    private class TestChoiceOptionNode : IChoiceOptionNode
    {
        public string Key { get; set; }

        public string Text { get; set; }

        public string DisabledText { get; set; }

        public string ImageName { get; set; }

        public string HelpText { get; set; }

        public ICondition EnabledWhen { get; set; }

        public ICondition VisibleWhen { get; set; }
    }
}

public static class IChoiceExtensionMethods
{
    public static void ShouldBe(this IChoiceOption option, string key, string text, bool isEnabled, string imageName, string helpText)
    {
        option.Key.Should().Be(key);
        option.Text.Should().Be(text);
        option.IsEnabled.Should().Be(isEnabled);
        option.ImageName.Should().Be(imageName);
        option.HelpText.Should().Be(helpText);
    }
}