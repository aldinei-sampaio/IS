using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class PauseNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly IParsingSceneContext sceneContext;
    private readonly FakeParentParsingContext parentParsingContext = new();
    private readonly IIntegerArgumentParser integerArgumentParser;
    private readonly PauseNodeParser sut;

    public PauseNodeParserTests()
    {
        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        sceneContext = A.Fake<IParsingSceneContext>(i => i.Strict());
        A.CallTo(() => parsingContext.SceneContext).Returns(sceneContext);

        integerArgumentParser = A.Fake<IIntegerArgumentParser>();

        sut = new(integerArgumentParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("pause");
        sut.IsArgumentRequired.Should().BeFalse();
        sut.IntegerArgumentParser.Should().BeSameAs(integerArgumentParser);
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "2000";

        A.CallTo(() => integerArgumentParser.Parse(argument, PauseNodeParser.MinTimeout, PauseNodeParser.MaxTimeout))
            .Returns(Result.Fail<int>(errorMessage));
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task WhenValidArgumentIsProvidedShouldGenerateATimedPauseNode()
    {
        var argument = "2000";

        A.CallTo(() => integerArgumentParser.Parse(argument, PauseNodeParser.MinTimeout, PauseNodeParser.MaxTimeout))
            .Returns(Result.Ok(2000));
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => sceneContext.Reset()).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<TimedPauseNode>(
            i => i.Duration.Should().Be(TimeSpan.FromMilliseconds(2000))
        );
        A.CallTo(() => sceneContext.Reset()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task WhenNoArgumentIsProvidedShouldGenerateAPauseNode()
    {
        A.CallTo(() => documentReader.Argument).Returns(string.Empty);
        A.CallTo(() => sceneContext.Reset()).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<PauseNode>();
        A.CallTo(() => sceneContext.Reset()).MustHaveHappenedOnceExactly();
    }
}
