using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceOptionImageNameSetterTests
{
    [Fact]
    public void Initialization()
    {
        var value = "x";
        var sut = new ChoiceOptionImageNameSetter(value);
        sut.Value.Should().Be(value);
    }

    [Fact]
    public void Build()
    {
        var value = "x";

        var navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        var prototype = A.Fake<IChoiceOptionPrototype>(i => i.Strict());
        A.CallToSet(() => prototype.ImageName).To(value).DoesNothing();

        var sut = new ChoiceOptionImageNameSetter(value);
        sut.Build(prototype, navigationContext);

        A.CallToSet(() => prototype.ImageName).To(value).MustHaveHappenedOnceExactly();
    }
}
