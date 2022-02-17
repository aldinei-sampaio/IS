using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceTimeLimitSetterTests
{
    [Fact]
    public void Initialization()
    {
        var value = TimeSpan.FromMilliseconds(1234);
        var sut = new ChoiceTimeLimitSetter(value);
        sut.Value.Should().Be(value);
    }

    [Fact]
    public void Build()
    {
        var value = TimeSpan.FromMilliseconds(1234);

        var navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        var prototype = A.Fake<IChoicePrototype>(i => i.Strict());
        A.CallToSet(() => prototype.TimeLimit).To(value).DoesNothing();

        var sut = new ChoiceTimeLimitSetter(value);
        sut.Build(prototype, navigationContext);

        A.CallToSet(() => prototype.TimeLimit).To(value).MustHaveHappenedOnceExactly();
    }
}
