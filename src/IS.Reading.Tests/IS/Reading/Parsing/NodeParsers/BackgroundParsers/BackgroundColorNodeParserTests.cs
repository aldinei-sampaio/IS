using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.State;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundColorNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext;

    private readonly IColorArgumentParser colorArgumentParser;
    private readonly BackgroundColorNodeParser sut;

    public BackgroundColorNodeParserTests()
    {
        colorArgumentParser = A.Fake<IColorArgumentParser>(i => i.Strict());
        sut = new(colorArgumentParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new FakeParentParsingContext();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("color");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ColorArgumentParser.Should().BeSameAs(colorArgumentParser);
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "salmon";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => colorArgumentParser.Parse(argument)).Returns(Result.Fail<string>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParsingSuccess()
    {
        var argument = "Gelo";
        var parsed = "gelo";

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => colorArgumentParser.Parse(argument)).Returns(Result.Ok(parsed));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        var expectedState = new BackgroundState(parsed, BackgroundType.Color, BackgroundPosition.Undefined);

        parentParsingContext.ShouldContainSingle<BackgroundNode>(
            i => i.State.Should().Be(expectedState)
        );
    }    
}
