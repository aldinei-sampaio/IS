using IS.Reading.Choices;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Parsing.NodeParsers.ChoiceParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class ChoiceNodeParserTests
{
    private readonly ChoiceNodeParser sut;
    private readonly IElementParser elementParser;
    private readonly IChoiceTimeLimitNodeParser timeLimitNodeParser;
    private readonly IChoiceDefaultNodeParser defaultNodeParser;
    private readonly IChoiceOptionNodeParser optionNodeParser;
    private readonly IChoiceRandomOrderNodeParser randomOrderNodeParser;
    private readonly IChoiceIfNodeParser ifNodeParser;
    private readonly INameArgumentParser nameArgumentParser;

    public ChoiceNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        timeLimitNodeParser = Helper.FakeParser<IChoiceTimeLimitNodeParser>("alpha");
        defaultNodeParser = Helper.FakeParser<IChoiceDefaultNodeParser>("beta");
        optionNodeParser = Helper.FakeParser<IChoiceOptionNodeParser>("gamma");
        randomOrderNodeParser = Helper.FakeParser<IChoiceRandomOrderNodeParser>("random");
        ifNodeParser = Helper.FakeParser<IChoiceIfNodeParser>("if");
        nameArgumentParser = A.Fake<INameArgumentParser>(i => i.Strict());

        sut = new(
            elementParser, 
            nameArgumentParser, 
            timeLimitNodeParser, 
            defaultNodeParser,
            randomOrderNodeParser,
            optionNodeParser,
            ifNodeParser
        );
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("?");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.NameArgumentParser.Should().BeSameAs(nameArgumentParser);
        sut.Settings.Should().BeOfType<ElementParserSettings.Block>();
        sut.Settings.ChildParsers.Should().BeEquivalentTo(
            timeLimitNodeParser, 
            defaultNodeParser, 
            randomOrderNodeParser, 
            optionNodeParser,
            ifNodeParser
        );
    }

    [Fact]
    public async Task ShouldLogAttributeParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "Gibberish";

        var reader = A.Fake<IDocumentReader>(i => i.Strict());
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new BalloonChildParsingContext(BalloonType.Speech);

        A.CallTo(() => reader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Fail<string>(errorMessage));
        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ChoiceBuilder.Should().BeNull();
        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnEventParserError()
    {
        var argument = "Gibberish";

        var reader = A.Fake<IDocumentReader>(i => i.Strict());
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new BalloonChildParsingContext(BalloonType.Speech);

        A.CallTo(() => reader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => context.IsSuccess).Returns(false);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ChoiceBuilder.Should().BeNull();
    }

    [Fact]
    public async Task ShouldExitWhenNoBuilderIsParsed()
    {
        var argument = "Gibberish";

        var reader = A.Fake<IDocumentReader>(i => i.Strict());
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new BalloonChildParsingContext(BalloonType.Speech);

        A.CallTo(() => reader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ChoiceBuilder.Should().BeNull();
    }

    [Fact]
    public async Task BuildersParsed()
    {
        var argument = "Gibberish";

        var reader = A.Fake<IDocumentReader>(i => i.Strict());
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new BalloonChildParsingContext(BalloonType.Speech);

        var builder1 = A.Dummy<IBuilder<IChoicePrototype>>();
        var builder2 = A.Dummy<IBuilder<IChoicePrototype>>();

        A.CallTo(() => reader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var ctx = i.GetChoiceContext();
                ctx.Builders.Add(builder1);
                ctx.Builders.Add(builder2);
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ChoiceBuilder.Should().BeOfType<ChoiceBuilder>()
            .Which.ShouldSatisfy(i =>
            {
                i.Key.Should().Be(argument);
                i.Items.Should().BeEquivalentTo(builder1, builder2);
            });
    }
}
