using IS.Reading.Nodes;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class TitleNodeParserTests
{
    private readonly ITextSourceParser textSourceParser;
    private readonly TitleNodeParser sut;

    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext;
    private readonly IParsingSceneContext parsingSceneContext;


    public TitleNodeParserTests()
    {
        textSourceParser = A.Fake<ITextSourceParser>(i => i.Strict());
        sut = new(textSourceParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        parentParsingContext = new();
        parsingSceneContext = A.Fake<IParsingSceneContext>(i => i.Strict());
        A.CallTo(() => parsingContext.SceneContext).Returns(parsingSceneContext);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("title");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.TextSourceParser.Should().BeSameAs(textSourceParser);
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "alabama";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Fail<ITextSource>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldCreateTitleNode()
    {
        var argument = "Calahan";
        var parsed = A.Dummy<ITextSource>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Ok(parsed));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<BalloonTitleNode>(i => i.TextSource.Should().Be(parsed));
    }
}
