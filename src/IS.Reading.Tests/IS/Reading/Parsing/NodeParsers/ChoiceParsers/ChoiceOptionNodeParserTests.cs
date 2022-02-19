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
    private readonly ChoiceParentParsingContext parentParsingContext;

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
        parentParsingContext = new ChoiceParentParsingContext();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be(string.Empty);
        sut.NameRegex.Should().Be("^[a-z]$");
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

        A.CallTo(() => documentReader.Command).Returns("a");
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Ok(textSource));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<ChoiceOptionBuilder>()
            .Which.Items.Should().ContainSingle()
            .Which.Should().BeOfType<ChoiceOptionTextSetter>()
            .Which.TextSource.Should().BeSameAs(textSource);
    }

    [Fact]
    public async Task ShouldExitOnElementParserError()
    {
        A.CallTo(() => documentReader.Command).Returns("a");
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
        A.CallTo(() => documentReader.Command).Returns("a");
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

        A.CallTo(() => documentReader.Command).Returns("a");
        A.CallTo(() => documentReader.Argument).Returns(string.Empty);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<ChoiceOptionParentParsingContext>(2).Builders.Add(builder));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<ChoiceOptionBuilder>()
            .Which.Items.Should().ContainSingle()
            .Which.Should().BeSameAs(builder);
    }
}
