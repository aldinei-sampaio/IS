using IS.Reading.Choices;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly ITextSourceParser textSourceParser;
    private readonly IChoiceOptionTextNodeParser choiceOptionTextNodeParser;
    private readonly IChoiceOptionDisabledNodeParser choiceOptionDisabledNodeParser;
    private readonly IChoiceOptionIconNodeParser choiceOptionIconNodeParser;
    private readonly IChoiceOptionTipNodeParser choiceOptionTipNodeParser;
    private readonly IChoiceOptionIfNodeParser choiceOptionIfNodeParser;
    private readonly ChoiceOptionNodeParser sut;

    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly BuilderParentParsingContext<IChoicePrototype> parentParsingContext;

    public ChoiceOptionNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        textSourceParser = A.Fake<ITextSourceParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        choiceOptionTextNodeParser = Helper.FakeParser<IChoiceOptionTextNodeParser>("a");
        choiceOptionDisabledNodeParser = Helper.FakeParser<IChoiceOptionDisabledNodeParser>("disabled");
        choiceOptionIconNodeParser = Helper.FakeParser<IChoiceOptionIconNodeParser>("icon");
        choiceOptionTipNodeParser = Helper.FakeParser<IChoiceOptionTipNodeParser>("tip");
        choiceOptionIfNodeParser = Helper.FakeParser<IChoiceOptionIfNodeParser>("if");

        sut = new(
            elementParser,
            textSourceParser,
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionTipNodeParser,
            choiceOptionIfNodeParser
        );

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be(string.Empty);
        sut.NameRegex.Should().Be(@"^[A-Za-z0-9_]+\)$");
        sut.IsArgumentRequired.Should().BeFalse();
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.TextSourceParser.Should().BeSameAs(textSourceParser);
        sut.Settings.Should().BeOfType<ElementParserSettings.Block>();
        sut.Settings.ChildParsers.Should().BeEquivalentTo(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionTipNodeParser,
            choiceOptionIfNodeParser
        );
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "Texto da opção";
        A.CallTo(() => documentReader.Command).Returns("a");
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Fail<ITextSource>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ArgumentParsingSuccess()
    {
        var argument = "Texto da opção";
        var textSource = A.Dummy<ITextSource>();

        A.CallTo(() => documentReader.Command).Returns("a)");
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Ok(textSource));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<ChoiceOptionBuilder>()
            .Which.Items.Should().ContainSingle()
            .Which.Should().BeOfType<ChoiceOptionTextSetter>()
            .Which.TextSource.Should().BeSameAs(textSource);
    }

    [Theory]
    [InlineData("a)", "a")]
    [InlineData("A)", "a")]
    [InlineData("b)", "b")]
    [InlineData("B)", "b")]
    [InlineData("y)", "y")]
    [InlineData("Y)", "y")]
    [InlineData("Z)", "z")]
    [InlineData("z)", "z")]
    [InlineData("0)", "0")]
    [InlineData("9)", "9")]
    [InlineData("Big_Value)", "big_value")]
    [InlineData("0123456789)", "0123456789")]
    public async Task OptionKeyParsing(string command, string key)
    {
        var argument = "Texto da opção";
        var textSource = A.Dummy<ITextSource>();

        A.CallTo(() => documentReader.Command).Returns(command);
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Ok(textSource));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<ChoiceOptionBuilder>()
            .Which.Key.Should().Be(key);
    }

    [Fact]
    public async Task ShouldExitOnElementParserError()
    {
        A.CallTo(() => documentReader.Command).Returns("a)");
        A.CallTo(() => documentReader.Argument).Returns(string.Empty);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();
        A.CallTo(() => parsingContext.IsSuccess).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldExitIfAggregationParsingDoesNotProduceBuilders()
    {
        A.CallTo(() => documentReader.Command).Returns("a)");
        A.CallTo(() => documentReader.Argument).Returns(string.Empty);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();
    }

    [Fact]
    public async Task AggregationParsingSuccess()
    {
        var builder = A.Dummy<IBuilder<IChoiceOptionPrototype>>();

        A.CallTo(() => documentReader.Command).Returns("a)");
        A.CallTo(() => documentReader.Argument).Returns(string.Empty);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetOptionContext().Builders.Add(builder));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<ChoiceOptionBuilder>()
            .Which.Items.Should().ContainSingle()
            .Which.Should().BeSameAs(builder);
    }
}
