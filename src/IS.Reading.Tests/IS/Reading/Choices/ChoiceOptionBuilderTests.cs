using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceOptionBuilderTests
{
    private readonly INavigationContext navigationContext;
    private readonly string key;

    public ChoiceOptionBuilderTests()
    {
        navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        key = "opcao";
    }

    [Fact]
    public void Initialization()
    {
        var builder1 = A.Fake<IBuilder<IChoiceOptionPrototype>>(i => i.Strict());
        var builder2 = A.Fake<IBuilder<IChoiceOptionPrototype>>(i => i.Strict());
        var items = new[] { builder1, builder2 };
        var sut = new ChoiceOptionBuilder(key, items);

        sut.Key.Should().Be(key);
        sut.Items.Should().BeSameAs(items);
    }

    [Fact]
    public void MustCallBuildOfEveryItem()
    {
        var builder1 = A.Fake<IBuilder<IChoiceOptionPrototype>>(i => i.Strict());
        var builder2 = A.Fake<IBuilder<IChoiceOptionPrototype>>(i => i.Strict());
        var items = new[] { builder1, builder2 };
        var sut = new ChoiceOptionBuilder(key, items);

        var choicePrototype = A.Fake<IChoicePrototype>(i => i.Strict());

        A.CallTo(() => builder1.Build(A<IChoiceOptionPrototype>.Ignored, navigationContext)).DoesNothing();
        A.CallTo(() => builder2.Build(A<IChoiceOptionPrototype>.Ignored, navigationContext)).DoesNothing();

        sut.Build(choicePrototype, navigationContext);

        A.CallTo(() => builder1.Build(A<IChoiceOptionPrototype>.Ignored, navigationContext)).MustHaveHappenedOnceExactly();
        A.CallTo(() => builder2.Build(A<IChoiceOptionPrototype>.Ignored, navigationContext)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void ShouldIgnoreOptionsWithoutText()
    {
        var builder1 = new TestBuilder(i => i.Text = null);
        var builder2 = new TestBuilder(i => i.Text = string.Empty);

        var items = new[] { builder1, builder2 };
        var sut = new ChoiceOptionBuilder(key, items);

        var choicePrototype = A.Fake<IChoicePrototype>(i => i.Strict());

        sut.Build(choicePrototype, navigationContext);

        A.CallTo(() => choicePrototype.Add(A<IChoiceOption>.Ignored)).MustNotHaveHappened();
    }

    [Theory]
    [InlineData("alpha", true, "beta", "gamma")]
    [InlineData("abacate", false, "melancia", "pera")]
    public void PropertiesSetByBuildersShouldReflectOnCreatedOption(string text, bool isEnabled, string imageName, string tip)
    {
        var items = new[]
        {
            new TestBuilder(i => i.Text = text),
            new TestBuilder(i => i.IsEnabled = isEnabled),
            new TestBuilder(i => i.ImageName = imageName),
            new TestBuilder(i => i.Tip = tip)
        };

        IChoiceOption option = null;

        var choicePrototype = A.Fake<IChoicePrototype>(i => i.Strict());
        A.CallTo(() => choicePrototype.Add(A<IChoiceOption>.Ignored))
            .Invokes(i => option = i.GetArgument<IChoiceOption>(0));

        var sut = new ChoiceOptionBuilder(key, items);

        sut.Build(choicePrototype, navigationContext);

        option.Should().BeEquivalentTo(new
        {
            Key = key,
            Text = text,
            IsEnabled = isEnabled,
            ImageName = imageName,
            Tip = tip
        });
    }

    private class TestBuilder : IBuilder<IChoiceOptionPrototype>
    {
        private readonly Action<IChoiceOptionPrototype> action;

        public TestBuilder(Action<IChoiceOptionPrototype> action)
            => this.action = action;

        public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
            => action.Invoke(prototype);
    }
}