using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class RootBlockParserTests
{
    private readonly XmlReader reader;
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
    private readonly RootBlockParser sut;

    public RootBlockParserTests()
    {
        reader = A.Dummy<XmlReader>();
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

        sut = new(
            elementParser, 
            musicNodeParser,
            backgroundNodeParser, 
            blockNodeParser,
            pauseNodeParser,
            protagonistNodeParser,
            personNodeParser,
            narrationNodeParser,
            tutorialNodeParser
        );
    }

    [Fact]
    public void Initialization()
    {
        sut.Settings.AttributeParsers.Count.Should().Be(0);
        sut.Settings.ChildParsers["music"].Should().BeSameAs(musicNodeParser);
        sut.Settings.ChildParsers["background"].Should().BeSameAs(backgroundNodeParser);
        sut.Settings.ChildParsers["do"].Should().BeSameAs(blockNodeParser);
        sut.Settings.ChildParsers["pause"].Should().BeSameAs(pauseNodeParser);
        sut.Settings.ChildParsers["protagonist"].Should().BeSameAs(protagonistNodeParser);
        sut.Settings.ChildParsers["person"].Should().BeSameAs(personNodeParser);
        sut.Settings.ChildParsers["narration"].Should().BeSameAs(narrationNodeParser);
        sut.Settings.ChildParsers["tutorial"].Should().BeSameAs(tutorialNodeParser);
        sut.Settings.ChildParsers.Count.Should().Be(8);
        sut.Settings.TextParser.Should().BeNull();
    }

    [Fact]
    public async Task SimpleParsing()
    {
        var parsed = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(parsed));

        var result = await sut.ParseAsync(reader, context);
        result.ForwardQueue.Dequeue().Should().BeSameAs(parsed);
        result.ForwardQueue.Count().Should().Be(0);
    }

    [Fact]
    public async Task Empty()
    {
        A.CallTo(() => context.LogError(reader, "Elemento filho era esperado.")).DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        var result = await sut.ParseAsync(reader, context);
        result.Should().BeNull();

        A.CallTo(() => context.LogError(reader, "Elemento filho era esperado.")).MustHaveHappenedOnceExactly();
    }
}
