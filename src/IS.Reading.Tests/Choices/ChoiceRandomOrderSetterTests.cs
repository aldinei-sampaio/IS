using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceRandomOrderSetterTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Initialization(bool value)
    {
        var sut = new ChoiceRandomOrderSetter(value);
        sut.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Build(bool value)
    {
        var navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        var prototype = A.Fake<IChoicePrototype>(i => i.Strict());
        A.CallToSet(() => prototype.RandomOrder).To(value).DoesNothing();

        var sut = new ChoiceRandomOrderSetter(value);
        sut.Build(prototype, navigationContext);

        A.CallToSet(() => prototype.RandomOrder).To(value).MustHaveHappenedOnceExactly();
    }
}