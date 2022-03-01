using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class BalloonTitleNodeTests
{
    private readonly IVariableDictionary variables;
    private readonly INavigationState state;
    private readonly INavigationContext context;
    private readonly ITextSource textSource;
    private readonly TestInvoker testInvoker;
    private readonly BalloonTitleNode sut;

    public BalloonTitleNodeTests()
    {
        variables = A.Fake<IVariableDictionary>(i => i.Strict());
        state = A.Fake<INavigationState>(i => i.Strict());
        context = A.Fake<INavigationContext>(i => i.Strict());
        A.CallTo(() => context.State).Returns(state);
        A.CallTo(() => context.Variables).Returns(variables);
        testInvoker = new TestInvoker(context);

        textSource = A.Dummy<ITextSource>();

        sut = new(textSource);
    }

    [Theory]
    [InlineData("Micalateia", null)]
    [InlineData("Dariana", "Aldebarda")]
    public async Task ShouldSetTitleToSpecifiedValue(string newValue, string oldValue)
    {
        A.CallTo(() => textSource.Build(variables)).Returns(newValue ?? string.Empty);
        A.CallTo(() => state.Title).Returns(oldValue);
        A.CallToSet(() => state.Title).To(newValue).DoesNothing();

        var ret = await sut.EnterAsync(context);

        ret.Should().Be(oldValue);
        A.CallToSet(() => state.Title).To(newValue).MustHaveHappened();

        testInvoker.ShouldHadReceived<IBalloonTitleEvent>(
            i => i.Text.Should().Be(newValue)
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Alpha")]
    [InlineData("Beta")]
    public async Task ShouldNotRaiseEventWhenTitleWasNotChanged(string title)
    {
        A.CallTo(() => textSource.Build(variables)).Returns(title ?? string.Empty);
        A.CallTo(() => state.Title).Returns(title);

        var ret = await sut.EnterAsync(context);
        
        ret.Should().Be(title);
        testInvoker.HadReceivedEvent.Should().BeFalse();
    }

    [Theory]
    [InlineData("Micalateia", null)]
    [InlineData("Dariana", "Aldebarda")]
    public async Task ShouldRaiseEventWithStateArg(string newValue, string oldValue)
    {
        A.CallTo(() => textSource.Build(variables)).Returns(newValue ?? string.Empty);
        A.CallTo(() => state.Title).Returns(oldValue);
        A.CallToSet(() => state.Title).To(newValue).DoesNothing();

        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context, newValue);

        A.CallToSet(() => state.Title).To(newValue).MustHaveHappenedOnceExactly();

        invoker.ShouldHadReceived<IBalloonTitleEvent>(
            i => i.Text.Should().Be(newValue)
        );
    }

    [Fact]
    public async Task ShouldNotRaiseEventWithStateArgNull()
    {
        string newValue = null;

        A.CallTo(() => state.Title).Returns("alpha");
        A.CallToSet(() => state.Title).To(newValue).DoesNothing();

        var invoker = new TestInvoker(context);

        await sut.EnterAsync(context, null);

        A.CallToSet(() => state.Title).To(newValue).MustHaveHappenedOnceExactly();
        testInvoker.HadReceivedEvent.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldNotRaiseEventWithEmptyNewValueAndNullOldValue()
    {
        var newValue = string.Empty;

        A.CallTo(() => state.Title).Returns("alpha");
        A.CallTo(() => state.Title).Returns(null);

        var invoker = new TestInvoker(context);

        var result = await sut.EnterAsync(context);

        result.Should().BeNull();
        testInvoker.HadReceivedEvent.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldNotRaiseEventWhenTextSourceIsNull()
    {
        var oldValue = "omega";
        string newValue = null;

        A.CallTo(() => state.Title).Returns(oldValue);
        A.CallToSet(() => state.Title).To(newValue).DoesNothing();

        var invoker = new TestInvoker(context);

        var sut = new BalloonTitleNode(null);
        var result = await sut.EnterAsync(context);

        result.Should().Be(oldValue);
        A.CallToSet(() => state.Title).To(newValue).MustHaveHappenedOnceExactly();
        testInvoker.HadReceivedEvent.Should().BeFalse();
    }
}
