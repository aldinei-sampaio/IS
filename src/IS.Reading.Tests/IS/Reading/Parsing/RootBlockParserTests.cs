using IS.Reading.Navigation;
using IS.Reading.Parsing.NodeParsers;

namespace IS.Reading.Parsing;

public class RootBlockParserTests
{
    private readonly IDocumentReader reader;
    private readonly IParsingContext context;
    private readonly IElementParser elementParser;
    private readonly IMusicNodeParser musicNodeParser;
    private readonly IBackgroundNodeParser backgroundNodeParser;
    private readonly IBlockNodeParser blockNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly IProtagonistNodeParser protagonistNodeParser;
    private readonly IPersonNodeParser personNodeParser;
    private readonly INarrationNodeParser narrationNodeParser;
    private readonly ITutorialNodeParser tutorialNodeParser;
    private readonly ISetNodeParser setNodeParser;
    private readonly RootBlockParser sut;

    public RootBlockParserTests()
    {
        reader = A.Dummy<IDocumentReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());

        backgroundNodeParser = Helper.FakeParser<IBackgroundNodeParser>("background");
        blockNodeParser = Helper.FakeParser<IBlockNodeParser>("do");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");
        musicNodeParser = Helper.FakeParser<IMusicNodeParser>("music");
        protagonistNodeParser = Helper.FakeParser<IProtagonistNodeParser>("protagonist");
        personNodeParser = Helper.FakeParser<IPersonNodeParser>("person");
        narrationNodeParser = Helper.FakeParser<INarrationNodeParser>("narration");
        tutorialNodeParser = Helper.FakeParser<ITutorialNodeParser>("tutorial");
        setNodeParser = Helper.FakeParser<ISetNodeParser>("set");

        sut = new(
            elementParser, 
            musicNodeParser,
            backgroundNodeParser, 
            blockNodeParser,
            pauseNodeParser,
            protagonistNodeParser,
            personNodeParser,
            narrationNodeParser,
            tutorialNodeParser,
            setNodeParser
        );
    }

    [Fact]
    public void Initialization()
    {
        sut.Settings.Should().BeOfType<ElementParserSettings.NoBlock>();
        sut.Settings.ChildParsers.Should().BeEquivalentTo(
            musicNodeParser,
            backgroundNodeParser,
            blockNodeParser,
            pauseNodeParser,
            protagonistNodeParser,
            personNodeParser,
            narrationNodeParser,
            tutorialNodeParser,
            setNodeParser
        );
    }

    [Fact]
    public async Task SimpleParsing()
    {
        var parsed = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(parsed));

        var result = await sut.ParseAsync(reader, context);
        result.Should().ContainSingle().Which.Should().BeSameAs(parsed);
    }

    [Fact]
    public async Task Empty()
    {
        A.CallTo(() => context.LogError(reader, "Elemento filho era esperado.")).DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        var result = await sut.ParseAsync(reader, context);
        result.Should().BeEmpty();

        A.CallTo(() => context.LogError(reader, "Elemento filho era esperado.")).MustHaveHappenedOnceExactly();
    }
}
