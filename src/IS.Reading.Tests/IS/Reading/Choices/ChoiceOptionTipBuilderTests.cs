using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Choices;

public class ChoiceOptionTipBuilderTests
{
    [Fact]
    public void Initialization()
    {
        var textSource = A.Fake<ITextSource>(i => i.Strict());
        var sut = new ChoiceOptionTipBuilder(textSource);
        sut.TextSource.Should().BeSameAs(textSource);
    }

    [Fact]
    public void Build()
    {
        var variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        var navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        A.CallTo(() => navigationContext.Variables).Returns(variableDictionary);

        var prototype = A.Fake<IChoiceOptionPrototype>(i => i.Strict());
        A.CallToSet(() => prototype.Tip).To("epsilon").DoesNothing();

        var textSource = A.Fake<ITextSource>(i => i.Strict());
        A.CallTo(() => textSource.Build(variableDictionary)).Returns("epsilon");

        var sut = new ChoiceOptionTipBuilder(textSource);

        sut.Build(prototype, navigationContext);

        A.CallToSet(() => prototype.Tip).To("epsilon").MustHaveHappenedOnceExactly();
    }
}