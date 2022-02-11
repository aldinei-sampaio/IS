using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Choices;

public class ChoiceOptionTextBuilderTests
{
    [Fact]
    public void Initialization()
    {
        var textSource = A.Fake<ITextSource>(i => i.Strict());
        var sut = new ChoiceOptionTextBuilder(textSource);
        sut.TextSource.Should().BeSameAs(textSource);
    }

    [Fact]
    public void Build()
    {
        var variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        var navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        A.CallTo(() => navigationContext.Variables).Returns(variableDictionary);
        
        var prototype = A.Fake<IChoiceOptionPrototype>(i => i.Strict());
        A.CallToSet(() => prototype.Text).To("omega").DoesNothing();

        var textSource = A.Fake<ITextSource>(i => i.Strict());
        A.CallTo(() => textSource.Build(variableDictionary)).Returns("omega");

        var sut = new ChoiceOptionTextBuilder(textSource);

        sut.Build(prototype, navigationContext);

        A.CallToSet(() => prototype.Text).To("omega").MustHaveHappenedOnceExactly();
    }
}
