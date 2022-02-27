using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;
public class InputNodeTests
{
    private readonly string key;
    private readonly IInputEvent @event;
    private readonly IInputBuilder inputBuilder;
    private readonly INavigationContext navigationContext;
    private readonly INavigationState navigationState;
    private readonly IVariableDictionary variableDictionary;
    private readonly InputNode sut;

    public InputNodeTests()
    {
        navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        navigationState = A.Fake<INavigationState>(i => i.Strict());
        variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        A.CallTo(() => navigationContext.State).Returns(navigationState);
        A.CallTo(() => navigationContext.Variables).Returns(variableDictionary);

        key = "delta";
        @event = A.Dummy<IInputEvent>();
        inputBuilder = A.Fake<IInputBuilder>(i => i.Strict());
        A.CallTo(() => inputBuilder.BuildEvent(variableDictionary)).Returns(@event);
        A.CallTo(() => inputBuilder.Key).Returns(key);

        sut = new InputNode(inputBuilder);
    }

    [Fact]
    public void Initialization()
    {
        sut.InputBuilder.Should().BeSameAs(inputBuilder);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abcde")]
    public async Task EnterAsync(string oldValue)
    {
        A.CallTo(() => variableDictionary[key]).Returns(oldValue);
        A.CallTo(() => navigationState.WaitingFor).Returns(null);
        A.CallToSet(() => navigationState.WaitingFor).To(key).DoesNothing();

        var invoker = new TestInvoker(navigationContext);

        var state = await sut.EnterAsync(navigationContext);

        state.Should().Be(oldValue);

        invoker.ShouldContainSingle<IInputEvent>(i => i.Should().BeSameAs(@event));
        A.CallToSet(() => navigationState.WaitingFor).To(key).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abcde")]
    public async Task EnterAsyncWithStateShouldRestoreVarValue(string oldValue)
    {
        A.CallTo(() => variableDictionary[key]).Returns(A.Dummy<string>());
        A.CallToSet(() => variableDictionary[key]).To(oldValue).DoesNothing();

        A.CallTo(() => navigationState.WaitingFor).Returns(null);
        A.CallToSet(() => navigationState.WaitingFor).To(key).DoesNothing();

        var invoker = new TestInvoker(navigationContext);

        await sut.EnterAsync(navigationContext, oldValue);

        invoker.ShouldContainSingle<IInputEvent>(i => i.Should().BeSameAs(@event));
        A.CallToSet(() => navigationState.WaitingFor).To(key).MustHaveHappenedOnceExactly();
        A.CallToSet(() => variableDictionary[key]).To(oldValue).MustHaveHappenedOnceExactly();
    }
}
