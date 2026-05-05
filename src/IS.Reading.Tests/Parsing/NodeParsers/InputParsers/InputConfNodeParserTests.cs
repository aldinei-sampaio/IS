using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.InputParsers;

public class InputConfNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly InputParentParsingContext parentParsingContext;

    private readonly ITextSourceParser textSourceParser;
    private readonly InputConfNodeParser sut;

    public InputConfNodeParserTests()
    {
        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        parentParsingContext = new("gama");

        textSourceParser = A.Fake<ITextSourceParser>(i => i.Strict());
        sut = new(textSourceParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("conf");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.TextSourceParser.Should().BeSameAs(textSourceParser);
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "Gibberish";
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Fail<ITextSource>(errorMessage));
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.InputBuilder.ConfirmationSource.Should().BeNull();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Success()
    {
        var argument = "Você informou '{0}'. Confirma?";
        var confirmationSource = A.Fake<ITextSource>(i => i.Strict());
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Ok(confirmationSource));
        A.CallTo(() => documentReader.Argument).Returns(argument);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.InputBuilder.ConfirmationSource.Should().BeSameAs(confirmationSource);
    }
}
