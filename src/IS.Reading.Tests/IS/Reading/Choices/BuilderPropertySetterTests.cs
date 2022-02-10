using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class BuilderPropertySetterTests
{
    public interface ITestProtype
    {
    }

    [Fact]
    public void Initialization()
    {
        var action = A.Fake<Action<ITestProtype>>(i => i.Strict());
        var sut = new BuilderPropertySetter<ITestProtype>(action);
        sut.BuildAction.Should().BeSameAs(action);
    }

    [Fact]
    public void Build()
    {
        var navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        var prototype = A.Fake<ITestProtype>(i => i.Strict());

        var action = A.Fake<Action<ITestProtype>>(i => i.Strict());
        A.CallTo(() => action.Invoke(prototype)).DoesNothing();

        var sut = new BuilderPropertySetter<ITestProtype>(action);
        
        sut.Build(prototype, navigationContext);

        A.CallTo(() => action.Invoke(prototype)).MustHaveHappenedOnceExactly();
    }
}
