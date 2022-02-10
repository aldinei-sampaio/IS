using IS.Reading.Choices;
using IS.Reading.Conditions;
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
        A.CallTo(() => textSource.ToString(variables)).Returns(balloonText);
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
        sut.ChoiceNode.Should().BeNull();
    }

    [Fact]
    public void InitializeChoiceNode()
    {
        var choiceNode = A.Dummy<IChoiceNode>();
        var sut = new BalloonTextNode(textSource, BalloonType.Narration, choiceNode);
        sut.ChoiceNode.Should().BeSameAs(choiceNode);
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

        var sut = new BalloonTextNode(textSource, BalloonType.Speech, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IBalloonTextEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                Text = balloonText,
                BalloonType = BalloonType.Speech,
                IsProtagonist = false,
                Choice = new
                {
                    TimeLimit = TimeSpan.FromSeconds(3),
                    Default = "a",
                    Options = new[]
                    {
                        new { Key = "a", Text = "Opção1", IsEnabled = true, ImageName = (string)null, HelpText = (string)null },
                        new { Key = "b", Text = "Opção2", IsEnabled = true, ImageName = "abc", HelpText = "help" }
                    }
                }
            })
        );
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

        var sut = new BalloonTextNode(textSource, BalloonType.Speech, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IBalloonTextEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                Text = balloonText,
                BalloonType = BalloonType.Speech,
                IsProtagonist = true,
                Choice = new
                {
                    TimeLimit = (TimeSpan?)null,
                    Default = (string)null,
                    Options = new[]
                    {
                        new { Key = "a", Text = "Opção1", IsEnabled = true, ImageName = (string)null, HelpText = (string)null },
                        new { Key = "b", Text = "Não ativa", IsEnabled = false, ImageName = (string)null, HelpText = (string)null },
                        new { Key = "c", Text = "Opção3", IsEnabled = false, ImageName = (string)null, HelpText = (string)null }
                    }
                }
            })
        );

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

        var sut = new BalloonTextNode(textSource, BalloonType.Thought, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IBalloonTextEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                Text = balloonText,
                BalloonType = BalloonType.Thought,
                IsProtagonist = true,
                Choice = new
                {
                    TimeLimit = (TimeSpan?)null,
                    Default = (string)null,
                    Options = new[]
                    {
                        new { Key = "b", Text = "Opção2", IsEnabled = true, ImageName = (string)null, HelpText = (string)null }
                    }
                }
            })
        );

        A.CallTo(() => visible1.Evaluate(variables)).MustHaveHappenedOnceExactly();
        A.CallTo(() => visible2.Evaluate(variables)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ChoiceMustBeNullWhenThereAreNoOptions()
    {
        var when = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => when.Evaluate(variables)).Returns(false);

        var choiceNode = new TestChoiceNode { Options = new() };

        var sut = new BalloonTextNode(textSource, BalloonType.Tutorial, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IBalloonTextEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                Text = balloonText,
                BalloonType = BalloonType.Tutorial,
                IsProtagonist = true,
                Choice = (IChoice)null
            })
        );
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

        var sut = new BalloonTextNode(textSource, BalloonType.Narration, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IBalloonTextEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                Text = balloonText,
                BalloonType = BalloonType.Narration,
                IsProtagonist = true,
                Choice = (IChoice)null
            })
        );
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

        var sut = new BalloonTextNode(textSource, BalloonType.Speech, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IBalloonTextEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                Text = balloonText,
                BalloonType = BalloonType.Speech,
                IsProtagonist = true,
                Choice = (IChoice)null
            })
        );
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

        var sut = new BalloonTextNode(textSource, BalloonType.Speech, choiceNode);
        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IBalloonTextEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                Text = balloonText,
                BalloonType = BalloonType.Speech,
                IsProtagonist = true,
                Choice = new
                {
                    TimeLimit = (TimeSpan?)null,
                    Default = (string)null,
                    Options = shuffled
                }
            })
        );
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
        option.Tip.Should().Be(helpText);
    }
}