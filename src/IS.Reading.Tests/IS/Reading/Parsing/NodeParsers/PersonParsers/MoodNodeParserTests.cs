using IS.Reading.Nodes;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class MoodNodeParserTests
{
    private readonly MoodNodeParser sut;

    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly IParsingSceneContext parsingSceneContext;
    private readonly FakeParentParsingContext parentParsingContext;

    public MoodNodeParserTests()
    {
        sut = new();

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new FakeParentParsingContext();
        parsingSceneContext = A.Fake<IParsingSceneContext>();
        A.CallTo(() => parsingContext.SceneContext).Returns(parsingSceneContext);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("#");
        sut.IsArgumentRequired.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldLogArgumentParsingError()
    {
        var errorMessage = "O valor 'Gibberish' não representa uma emoção válida.";
        var argument = "Gibberish";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldLogErrorWhenSceneAlreadyHaveMood()
    {
        var errorMessage = "Mais de uma definição de humor para a mesma cena.";

        A.CallTo(() => documentReader.Argument).Returns("Happy");
        A.CallTo(() => parsingSceneContext.HasMood).Returns(true);
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParseAsyncShouldReturnMoodNode()
    {
        A.CallTo(() => documentReader.Argument).Returns("Happy");
        A.CallTo(() => parsingSceneContext.HasMood).Returns(false);
        A.CallToSet(() => parsingSceneContext.HasMood).To(true).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<MoodNode>(i => i.MoodType.Should().Be(MoodType.Happy));
        A.CallToSet(() => parsingSceneContext.HasMood).To(true).MustHaveHappenedOnceExactly();
    }
}
