using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceBuilderTests
{
    private readonly INavigationContext navigationContext;
    private readonly string key;

    public ChoiceBuilderTests()
    {
        navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        key = "variavel";
    }

    [Fact]
    public void Initialization()
    {
        var builder1 = A.Fake<IBuilder<IChoicePrototype>>(i => i.Strict());
        var builder2 = A.Fake<IBuilder<IChoicePrototype>>(i => i.Strict());
        var items = new[] { builder1, builder2 };
        var sut = new ChoiceBuilder(key, items);

        sut.Key.Should().Be(key);
        sut.Items.Should().BeSameAs(items);
    }

    [Fact]
    public void MustCallBuildOfEveryItem()
    {
        var builder1 = A.Fake<IBuilder<IChoicePrototype>>(i => i.Strict());
        var builder2 = A.Fake<IBuilder<IChoicePrototype>>(i => i.Strict());
        var items = new[] { builder1, builder2 };
        var sut = new ChoiceBuilder(key, items);

        A.CallTo(() => builder1.Build(A<IChoicePrototype>.Ignored, navigationContext)).DoesNothing();
        A.CallTo(() => builder2.Build(A<IChoicePrototype>.Ignored, navigationContext)).DoesNothing();

        sut.Build(navigationContext);

        A.CallTo(() => builder1.Build(A<IChoicePrototype>.Ignored, navigationContext)).MustHaveHappenedOnceExactly();
        A.CallTo(() => builder2.Build(A<IChoicePrototype>.Ignored, navigationContext)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void ShouldReturnNullIfNoOptionWasProvidedByTheBuilders()
    {
        var builder1 = A.Fake<IBuilder<IChoicePrototype>>(i => i.Strict());
        var builder2 = A.Fake<IBuilder<IChoicePrototype>>(i => i.Strict());
        var items = new[] { builder1, builder2 };
        var sut = new ChoiceBuilder(key, items);

        A.CallTo(() => builder1.Build(A<IChoicePrototype>.Ignored, navigationContext)).DoesNothing();
        A.CallTo(() => builder2.Build(A<IChoicePrototype>.Ignored, navigationContext)).DoesNothing();

        var result = sut.Build(navigationContext);
        result.Should().BeNull();
    }

    [Fact]
    public void ShouldReturnNullIfNoOptionProvidedIsEnabled()
    {
        var option1 = A.Fake<IChoiceOption>(i => i.Strict());
        A.CallTo(() => option1.IsEnabled).Returns(false);

        var option2 = A.Fake<IChoiceOption>(i => i.Strict());
        A.CallTo(() => option2.IsEnabled).Returns(false);

        var builder1 = new TestBuilder(i => i.Add(option1));
        var builder2 = new TestBuilder(i => i.Add(option2));
        var sut = new ChoiceBuilder(key, new[] { builder1, builder2 });

        var result = sut.Build(navigationContext);
        result.Should().BeNull();
    }

    [Fact]
    public void ShouldShuffleOptionsIfRandomOrderPropertyWasSet()
    {
        var shuffled = A.Fake<IEnumerable<IChoiceOption>>(i => i.Strict());

        var randomizer = A.Fake<IRandomizer>(i => i.Strict());
        A.CallTo(() => randomizer.Shuffle(A<IEnumerable<IChoiceOption>>.Ignored)).Returns(shuffled);            

        A.CallTo(() => navigationContext.Randomizer).Returns(randomizer);

        var option1 = A.Fake<IChoiceOption>(i => i.Strict());
        A.CallTo(() => option1.IsEnabled).Returns(true);

        var builder1 = new TestBuilder(i => i.Add(option1));
        var builder2 = new TestBuilder(i => i.RandomOrder = true);
        var sut = new ChoiceBuilder(key, new[] { builder1, builder2 });

        var result = sut.Build(navigationContext);
        result.Options.Should().BeSameAs(shuffled);

        A.CallTo(() => randomizer.Shuffle(A<IEnumerable<IChoiceOption>>.Ignored)).MustHaveHappenedOnceExactly();
    }

    private class TestBuilder : IBuilder<IChoicePrototype>
    {
        private readonly Action<IChoicePrototype> action;

        public TestBuilder(Action<IChoicePrototype> action)
            => this.action = action;

        public void Build(IChoicePrototype prototype, INavigationContext context)
            => action.Invoke(prototype);
    }

    [Fact]
    public void PropertiesSetByBuildersShouldReflectOnReturnedObject()
    {
        var option1 = A.Fake<IChoiceOption>(i => i.Strict());
        A.CallTo(() => option1.IsEnabled).Returns(true);

        var builder1 = new TestBuilder(i => i.Default = "a");
        var builder2 = new TestBuilder(i => i.TimeLimit = TimeSpan.FromSeconds(2));
        var builder3 = new TestBuilder(i => i.Add(option1));

        var sut = new ChoiceBuilder(key, new[] { builder1, builder2, builder3 });

        var result = sut.Build(navigationContext);

        result.Should().BeEquivalentTo(new
        {
            Key = key,
            Default = "a",
            TimeLimit = TimeSpan.FromSeconds(2),
            Options = new[] { option1 }
        });
    }
}