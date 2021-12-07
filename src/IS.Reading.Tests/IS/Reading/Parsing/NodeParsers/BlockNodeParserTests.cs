using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class BlockNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly FakeParentParsingContext parentContext = new();
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly IWhileAttributeParser whileAttributeParser;
    private readonly IMusicNodeParser musicNodeParser;
    private readonly IBackgroundNodeParser backgroundNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly IProtagonistNodeParser protagonistNodeParser;
    private readonly IPersonNodeParser personNodeParser;
    private readonly INarrationNodeParser narrationNodeParser;
    private readonly ITutorialNodeParser tutorialNodeParser;
    private readonly BlockNodeParser sut;

    public BlockNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();

        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = Helper.FakeParser<IWhenAttributeParser>("when");
        whileAttributeParser = Helper.FakeParser<IWhileAttributeParser>("while");
        musicNodeParser = Helper.FakeParser<IMusicNodeParser>("music");
        backgroundNodeParser = Helper.FakeParser<IBackgroundNodeParser>("background");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");
        protagonistNodeParser = Helper.FakeParser<IProtagonistNodeParser>("protagonist");
        personNodeParser = Helper.FakeParser<IPersonNodeParser>("person");
        narrationNodeParser = Helper.FakeParser<INarrationNodeParser>("narration");
        tutorialNodeParser = Helper.FakeParser<ITutorialNodeParser>("tutorial");

        sut = new(
            elementParser, 
            whenAttributeParser, 
            whileAttributeParser, 
            musicNodeParser,
            backgroundNodeParser, 
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
        sut.Name.Should().Be("do");
        sut.Settings.ShouldBeNormal(
            whenAttributeParser,
            whileAttributeParser,
            musicNodeParser,
            backgroundNodeParser,
            sut,
            pauseNodeParser,
            protagonistNodeParser,
            personNodeParser,
            tutorialNodeParser,
            narrationNodeParser
        );
    }

    [Fact]
    public async Task SimpleParsing()
    {
        var when = A.Dummy<ICondition>();
        var @while = A.Dummy<ICondition>();
        var parsedNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i =>
            {
                var ctx = i.Arguments.Get<IParentParsingContext>(2);
                ctx.When = when;
                ctx.While = @while;
                ctx.AddNode(parsedNode);
            });

        await sut.ParseAsync(reader, context, parentContext);

        var node = parentContext.ShouldContainSingle<BlockNode>();
        node.When.Should().BeSameAs(when);
        node.While.Should().BeSameAs(@while);
        node.ChildBlock.ForwardQueue.Count.Should().Be(1);
        node.ChildBlock.ForwardQueue.Peek().Should().BeSameAs(parsedNode);
    }

    [Fact]
    public async Task Empty()
    {
        A.CallTo(() => context.LogError(reader, "Elemento filho era esperado.")).DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);
        parentContext.ShouldBeEmpty();

        A.CallTo(() => context.LogError(reader, "Elemento filho era esperado.")).MustHaveHappenedOnceExactly();
    }
}
