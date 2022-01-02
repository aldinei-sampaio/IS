using IS.Reading.Navigation;
using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class BalloonTextNodeParserBaseTests
{
    private class TestClass : BalloonTextNodeParserBase
    {
        public TestClass(
            IElementParser elementParser, 
            IBalloonChildNodeParser balloonTextChildNodeParser,
            ISetNodeParser setNodeParser,
            IUnsetNodeParser unsetNodeParser
        ) 
            : base(elementParser, balloonTextChildNodeParser, setNodeParser, unsetNodeParser)
        {
        }
    }

    private readonly IElementParser elementParser;
    private readonly IBalloonChildNodeParser balloonTextChildNodeParser;
    private readonly ISetNodeParser setNodeParser;
    private readonly IUnsetNodeParser unsetNodeParser;
    private readonly TestClass sut;

    public BalloonTextNodeParserBaseTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        balloonTextChildNodeParser = Helper.FakeParser<IBalloonChildNodeParser>("abc");
        A.CallTo(() => balloonTextChildNodeParser.BalloonType).Returns(BalloonType.Speech);
        setNodeParser = Helper.FakeParser<ISetNodeParser>("set");
        unsetNodeParser = Helper.FakeParser<IUnsetNodeParser>("unset");

        sut = new(elementParser, balloonTextChildNodeParser, setNodeParser, unsetNodeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("abc");
        sut.Settings.ShouldBeAggregatedNonRepeat(balloonTextChildNodeParser);
        sut.AggregationSettings.ShouldBeAggregated(balloonTextChildNodeParser, setNodeParser, unsetNodeParser);
    }

    [Fact]
    public async Task NoAggregation()
    {
        var parentContext = new FakeParentParsingContext();
        var context = A.Dummy<IParsingContext>();
        var reader = A.Dummy<XmlReader>();
        A.CallTo(() => reader.ReadState).Returns(ReadState.Interactive);

        var dummyNode1 = A.Dummy<IPauseNode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(dummyNode1));

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<BalloonNode>(i =>
        {
            i.BallonType.Should().Be(BalloonType.Speech);
            i.ChildBlock.ShouldContainOnly(dummyNode1);
        });

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Aggregation()
    {
        var parentContext = new FakeParentParsingContext();
        var context = A.Dummy<IParsingContext>();
        var reader = A.Dummy<XmlReader>();
        A.CallTo(() => reader.ReadState).Returns(ReadState.Interactive);

        var dummyNode1 = A.Dummy<IPauseNode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(dummyNode1));

        var dummyNode2 = A.Dummy<IPauseNode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(dummyNode2));

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<BalloonNode>(i =>
        {
            i.BallonType.Should().Be(BalloonType.Speech);
            i.ChildBlock.ShouldBeEquivalentTo(dummyNode1, dummyNode2);
        });

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task NonPauseNodesShouldNotBeAtEndOfBalloonNode()
    {
        var parentContext = new FakeParentParsingContext();
        var context = A.Dummy<IParsingContext>();
        var reader = A.Dummy<XmlReader>();
        A.CallTo(() => reader.ReadState).Returns(ReadState.Interactive);

        var dummyNode1 = A.Dummy<IPauseNode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(dummyNode1));

        var dummyNode2 = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(dummyNode2));

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.Nodes.Count.Should().Be(2);
        parentContext.Nodes[0].Should().BeOfType<BalloonNode>().Which.ShouldSatisfy(i =>
        {
            i.BallonType.Should().Be(BalloonType.Speech);
            i.ChildBlock.ShouldContainOnly(dummyNode1);
        });
        parentContext.Nodes[1].Should().BeSameAs(dummyNode2);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldNotTryAggregateAtEndOfElement()
    {
        var parentContext = new FakeParentParsingContext();
        var context = A.Dummy<IParsingContext>();
        var reader = A.Dummy<XmlReader>();
        A.CallTo(() => reader.ReadState).Returns(ReadState.Interactive);

        var dummyNode1 = A.Dummy<IPauseNode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                i.Arguments.Get<IParentParsingContext>(2).AddNode(dummyNode1);
                A.CallTo(() => reader.ReadState).Returns(ReadState.EndOfFile);
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<BalloonNode>(i =>
        {
            i.BallonType.Should().Be(BalloonType.Speech);
            i.ChildBlock.ShouldContainOnly(dummyNode1);
        });

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldLogErrorWhenChildrenDoesNotCreateAnyNode()
    {
        const string message = "Era esperado um elemento filho.";

        var reader = A.Dummy<XmlReader>();
        A.CallTo(() => reader.ReadState).Returns(ReadState.Interactive);

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var parentContext = A.Fake<IParentParsingContext>(i => i.Strict());

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => context.LogError(reader, message))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .MustHaveHappenedOnceExactly();
    }
}
