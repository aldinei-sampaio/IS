using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class MusicNodeParserTests
{
    private readonly INameArgumentParser nameArgumentParser;
    private readonly MusicNodeParser sut;

    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext;
    private readonly IParsingSceneContext parsingSceneContext;


    public MusicNodeParserTests()
    {
        nameArgumentParser = A.Fake<INameArgumentParser>(i => i.Strict());
        sut = new(nameArgumentParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        parentParsingContext = new();
        parsingSceneContext = A.Fake<IParsingSceneContext>(i => i.Strict());
        A.CallTo(() => parsingContext.SceneContext).Returns(parsingSceneContext);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("music");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.NameArgumentParser.Should().BeSameAs(nameArgumentParser);
        sut.DismissNode.Should().BeOfType<MusicNode>()
            .Which.MusicName.Should().BeNull();
    }

    [Fact]
    public async Task ShouldLogNameParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "alabama";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Fail<string>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldLogErrorWhenCurrentSceneAlreadyHasAMusicChange()
    {
        var errorMessage = "Mais de uma definição de música para a mesma cena.";
        var argument = "open_sky";

        A.CallTo(() => parsingSceneContext.HasMusic).Returns(true);
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task OnSuccessShouldUpdateSceneContextAndRegisterDismissNode()
    {
        var argument = "Open_Sky";
        var parsed = "open_sky";

        A.CallTo(() => parsingSceneContext.HasMusic).Returns(false);
        A.CallToSet(() => parsingSceneContext.HasMusic).To(true).DoesNothing();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(parsed));
        A.CallTo(() => parsingContext.RegisterDismissNode(sut.DismissNode)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<MusicNode>(i => i.MusicName.Should().Be(parsed));
        A.CallToSet(() => parsingSceneContext.HasMusic).To(true).MustHaveHappenedOnceExactly();
        A.CallTo(() => parsingContext.RegisterDismissNode(sut.DismissNode)).MustHaveHappenedOnceExactly();
    }
}
