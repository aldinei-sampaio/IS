using IS.Reading.Events;
using IS.Reading.State;
using IS.Reading.Variables;

namespace IS.Reading.Navigation;

public class StoryboardTests
{
    private readonly ISceneNavigator sceneNavigator;
    private readonly IEventManager eventManager;
    private readonly INavigationContext navigationContext;
    private readonly INavigationState navigationState;
    private readonly Storyboard sut;

    public StoryboardTests()
    {
        sceneNavigator = A.Fake<ISceneNavigator>(i => i.Strict());
        eventManager = A.Fake<IEventManager>(i => i.Strict());
        navigationState = A.Fake<INavigationState>(i => i.Strict());
        navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        A.CallTo(() => navigationContext.State).Returns(navigationState);
        sut = new Storyboard(navigationContext, sceneNavigator, eventManager);
    }

    [Fact]
    public void Initialization()
    {
        sut.NavigationContext.Should().BeSameAs(navigationContext);
        sut.Events.Should().BeSameAs(eventManager);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task MoveAsync(bool forward, bool moveResult)
    {
        A.CallTo(() => navigationState.WaitingFor).Returns(null);
        A.CallTo(() => sceneNavigator.MoveAsync(sut.NavigationContext, forward)).Returns(moveResult);

        var result = await sut.MoveAsync(forward);
        result.Should().Be(moveResult);

        A.CallTo(() => sceneNavigator.MoveAsync(sut.NavigationContext, forward)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void DisposeShouldUnsubscribeEvents()
    {
        A.CallTo(() => eventManager.Dispose()).DoesNothing();
        sut.Dispose();
        A.CallTo(() => eventManager.Dispose()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task WaitingForShouldCauseExceptionWhenMovingForward()
    {
        A.CallTo(() => navigationState.WaitingFor).Returns("test");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.MoveAsync(true)
        );

        ex.Message.Should().Be("Valor de escolha é requerido para prosseguir.");
    }

    [Fact]
    public async Task WaitingForShouldBeClearedWhenMovingBackwards()
    {
        var forward = false;
        var moveResult = true;

        A.CallTo(() => navigationState.WaitingFor).Returns("alabama");
        A.CallToSet(() => navigationState.WaitingFor).To((string)null).DoesNothing();
        A.CallTo(() => sceneNavigator.MoveAsync(sut.NavigationContext, forward)).Returns(moveResult);

        var result = await sut.MoveAsync(forward);
        result.Should().Be(moveResult);

        A.CallTo(() => sceneNavigator.MoveAsync(sut.NavigationContext, forward)).MustHaveHappenedOnceExactly();
        A.CallToSet(() => navigationState.WaitingFor).To((string)null).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void SetChoiceShouldThrowExceptionIfWaitingForIsNull()
    {
        A.CallTo(() => navigationState.WaitingFor).Returns(null);

        var ex = Assert.Throws<InvalidOperationException>(
            () => sut.Input("test")
        );

        ex.Message.Should().Be("Valor de escolha não é esperado neste momento.");
    }

    [Fact]
    public void SetChoiceSuccess()
    {
        var variableName = "ping";
        var variableValue = "pong";

        var variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        A.CallTo(() => navigationContext.Variables).Returns(variableDictionary);
        A.CallToSet(() => variableDictionary[variableName]).To(variableValue).DoesNothing();        
        A.CallTo(() => navigationState.WaitingFor).Returns(variableName);
        A.CallToSet(() => navigationState.WaitingFor).To((string)null).DoesNothing();

        sut.Input(variableValue);

        A.CallToSet(() => variableDictionary[variableName]).To(variableValue).MustHaveHappenedOnceExactly();
        A.CallToSet(() => navigationState.WaitingFor).To((string)null).MustHaveHappenedOnceExactly();
    }
}
