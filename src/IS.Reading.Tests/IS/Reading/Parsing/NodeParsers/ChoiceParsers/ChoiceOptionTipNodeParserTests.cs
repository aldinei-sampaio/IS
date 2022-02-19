using IS.Reading.Choices;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionTipNodeParserTests
{
    private readonly ITextSourceParser textSourceParser;
    private readonly ChoiceOptionTipNodeParser sut;    

    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly ChoiceOptionParentParsingContext parentParsingContext;

    public ChoiceOptionTipNodeParserTests()
    {
        textSourceParser = A.Fake<ITextSourceParser>(i => i.Strict());
        sut = new(textSourceParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new ChoiceOptionParentParsingContext();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("tip");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.TextSourceParser.Should().BeSameAs(textSourceParser);
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "Texto da opção";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Fail<ITextSource>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParsingSuccess()
    {
        var argument = "Texto da opção";
        var parsed = A.Dummy<ITextSource>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Ok(parsed));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<ChoiceOptionTipSetter>()
            .Which.TextSource.Should().BeSameAs(parsed);
    }
}
