using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceDefaultSetterTests
{
    [Fact]
    public void Initialization()
    {
        var value = "x";
        var sut = new ChoiceDefaultSetter(value);
        sut.Value.Should().Be(value);
    }

    [Fact]
    public void Build()
    {
        var value = "x";

        var navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        var prototype = A.Fake<IChoicePrototype>(i => i.Strict());
        A.CallToSet(() => prototype.Default).To(value).DoesNothing();

        var sut = new ChoiceDefaultSetter(value);
        sut.Build(prototype, navigationContext);

        A.CallToSet(() => prototype.Default).To(value).MustHaveHappenedOnceExactly();
    }
}


