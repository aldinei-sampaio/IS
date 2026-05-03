using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceOptionIsEnabledSetterTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Initialization(bool value)
    {
        var sut = new ChoiceOptionIsEnabledSetter(value);
        sut.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Build(bool value)
    {
        var navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        var prototype = A.Fake<IChoiceOptionPrototype>(i => i.Strict());
        A.CallToSet(() => prototype.IsEnabled).To(value).DoesNothing();

        var sut = new ChoiceOptionIsEnabledSetter(value);
        sut.Build(prototype, navigationContext);

        A.CallToSet(() => prototype.IsEnabled).To(value).MustHaveHappenedOnceExactly();
    }
}
