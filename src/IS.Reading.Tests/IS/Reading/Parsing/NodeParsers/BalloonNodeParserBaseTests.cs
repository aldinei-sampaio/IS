using IS.Reading.Navigation;
using IS.Reading.Nodes;

namespace IS.Reading.Parsing.NodeParsers;

public class BalloonNodeParserBaseTests
{
    private class TestImplementation : BalloonNodeParserBase
    {
        public TestImplementation(IElementParser elementParser, params INodeParser[] parsers) : base(elementParser, parsers)
        {
        }

        public override string Name => "teste";

        public override BalloonType BalloonType => BalloonType.Speech;
    }

    private readonly IDocumentReader reader;
    private readonly IElementParser elementParser;
    private readonly IParsingContext context;
    private readonly FakeParentParsingContext parentContext;
    private readonly INodeParser parser1;
    private readonly INodeParser parser2;
    private readonly IBlockFactory blockFactory;
    private readonly TestImplementation sut;

    public BalloonNodeParserBaseTests()
    {
        elementParser = A.Dummy<IElementParser>();
        parser1 = Helper.FakeParser<INodeParser>("alpha");
        parser2 = Helper.FakeParser<INodeParser>("beta");
        sut = new TestImplementation(elementParser, parser1, parser2);

        context = A.Fake<IParsingContext>(i => i.Strict());
        parentContext = new FakeParentParsingContext();
        reader = A.Dummy<IDocumentReader>();
        blockFactory = new FakeBlockFactory();
        A.CallTo(() => context.BlockFactory).Returns(blockFactory);
    }


    [Fact]
    public void Initialization()
    {
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.Name.Should().Be("teste");
        sut.BalloonType.Should().Be(BalloonType.Speech);
        sut.Settings.Should().BeOfType<ElementParserSettings.Aggregated>()
            .Which.ChildParsers.Should().BeEquivalentTo(parser1, parser2);
    }

    [Fact]
    public async Task ExitOnElementParserError()
    {
        A.CallTo(() => context.IsSuccess).Returns(false);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldBeEmpty();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task NoAggregatedNodesFound()
    {
        var errorMessage = "Era esperada ao menos uma linha de diálogo após o comando 'teste'.";

        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task SimplePauseNode()
    {
        var pauseNode = A.Dummy<IPauseNode>();

        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).AddNode(pauseNode));

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<BalloonNode>(i =>
        {
            i.BallonType.Should().Be(BalloonType.Speech);
            i.ChildBlock.ShouldContainOnly(pauseNode);
        });
    }

    [Fact]
    public async Task MultiplePauseNodes()
    {
        var pauseNode1 = A.Dummy<IPauseNode>();
        var pauseNode2 = A.Dummy<IPauseNode>();
        var pauseNode3 = A.Dummy<IPauseNode>();

        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var parentContext = i.GetArgument<IParentParsingContext>(2);
                parentContext.AddNode(pauseNode1);
                parentContext.AddNode(pauseNode2);
                parentContext.AddNode(pauseNode3);
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<BalloonNode>(i =>
        {
            i.BallonType.Should().Be(BalloonType.Speech);
            i.ChildBlock.ShouldBeEquivalentTo(pauseNode1, pauseNode2, pauseNode3);
        });
    }

    [Fact]
    public async Task MultiplePauseAndNonPauseNodes()
    {
        var pauseNode1 = A.Dummy<IPauseNode>();
        var pauseNode2 = A.Dummy<IPauseNode>();
        var pauseNode3 = A.Dummy<IPauseNode>();
        var nonPauseNode1 = A.Dummy<INode>();
        var nonPauseNode2 = A.Dummy<INode>();
        var nonPauseNode3 = A.Dummy<INode>();

        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var parentContext = i.GetArgument<IParentParsingContext>(2);
                parentContext.AddNode(pauseNode1);
                parentContext.AddNode(nonPauseNode1);
                parentContext.AddNode(nonPauseNode2);
                parentContext.AddNode(pauseNode2);
                parentContext.AddNode(nonPauseNode3);
                parentContext.AddNode(pauseNode3);
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<BalloonNode>(i =>
        {
            i.BallonType.Should().Be(BalloonType.Speech);
            i.ChildBlock.ShouldBeEquivalentTo(
                pauseNode1, 
                nonPauseNode1,
                nonPauseNode2,
                pauseNode2, 
                nonPauseNode3,
                pauseNode3
            );
        });
    }

    [Fact]
    public async Task OnlyNonPauseNodes()
    {
        var errorMessage = "Era esperada ao menos uma linha de diálogo após o comando 'teste'.";
        
        var nonPauseNode1 = A.Dummy<INode>();
        var nonPauseNode2 = A.Dummy<INode>();

        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var parentContext = i.GetArgument<IParentParsingContext>(2);
                parentContext.AddNode(nonPauseNode1);
                parentContext.AddNode(nonPauseNode2);
            });

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task NonPauseNodesAfterLastPauseNode()
    {
        var pauseNode1 = A.Dummy<IPauseNode>();
        var pauseNode2 = A.Dummy<IPauseNode>();
        var pauseNode3 = A.Dummy<IPauseNode>();
        var nonPauseNode1 = A.Dummy<INode>();
        var nonPauseNode2 = A.Dummy<INode>();
        var nonPauseNode3 = A.Dummy<INode>();

        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var parentContext = i.GetArgument<IParentParsingContext>(2);
                parentContext.AddNode(pauseNode1);
                parentContext.AddNode(pauseNode2);
                parentContext.AddNode(nonPauseNode1);
                parentContext.AddNode(pauseNode3);
                parentContext.AddNode(nonPauseNode2);
                parentContext.AddNode(nonPauseNode3);
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContain(
            i => i.Should().BeOfType<BalloonNode>().Which.ShouldSatisfy(i =>
            {
                i.BallonType.Should().Be(BalloonType.Speech);
                i.ChildBlock.ShouldBeEquivalentTo(pauseNode1, pauseNode2, nonPauseNode1, pauseNode3);
            }, "Nodes[0]"),
            i => i.Should().BeSameAs(nonPauseNode2),
            i => i.Should().BeSameAs(nonPauseNode3)
        );
    }
}
