using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.InputParsers;

public class InputLenNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly InputParentParsingContext parentParsingContext;

    private readonly IIntegerArgumentParser integerArgumentParser;
    private readonly InputLenNodeParser sut;

    public InputLenNodeParserTests()
    {
        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        parentParsingContext = new("omicron");

        integerArgumentParser = A.Fake<IIntegerArgumentParser>(i => i.Strict());
        sut = new(integerArgumentParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("len");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.IntegerArgumentParser.Should().BeSameAs(integerArgumentParser);
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "Gibberish";
        A.CallTo(() => integerArgumentParser.Parse(argument, InputLenNodeParser.MinValue, InputLenNodeParser.MaxValue))
            .Returns(Result.Fail<int>(errorMessage));
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.InputBuilder.MaxLength.Should().Be(InputBuilder.MaxLenghtDefaultValue);
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Success()
    {
        var argument = "14";
        var maxLength = 14;
        A.CallTo(() => integerArgumentParser.Parse(argument, InputLenNodeParser.MinValue, InputLenNodeParser.MaxValue))
            .Returns(Result.Ok(maxLength));
        A.CallTo(() => documentReader.Argument).Returns(argument);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.InputBuilder.MaxLength.Should().Be(maxLength);
    }
}
