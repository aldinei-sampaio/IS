using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.State;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundRightNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext;

    private readonly IImageArgumentParser imageArgumentParser;
    private readonly BackgroundRightNodeParser sut;

    public BackgroundRightNodeParserTests()
    {
        imageArgumentParser = A.Fake<IImageArgumentParser>(i => i.Strict());
        sut = new(imageArgumentParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new FakeParentParsingContext();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("right");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ImageArgumentParser.Should().BeSameAs(imageArgumentParser); ;
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "forest";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => imageArgumentParser.Parse(argument)).Returns(Result.Fail<string>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParsingSuccess()
    {
        var argument = "Forest";
        var parsed = "forest";

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => imageArgumentParser.Parse(argument)).Returns(Result.Ok(parsed));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        var expectedState = new BackgroundState(parsed, BackgroundType.Image, BackgroundPosition.Right);

        parentParsingContext.ShouldContainSingle<BackgroundNode>(
            i => i.State.Should().Be(expectedState)
        );
    }
}
