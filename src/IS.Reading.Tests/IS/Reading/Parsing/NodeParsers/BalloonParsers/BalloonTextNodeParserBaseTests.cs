using IS.Reading.Navigation;
using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class BalloonTextNodeParserBaseTests
{
    private class TestClass : BalloonTextNodeParserBase
    {
        public TestClass(IElementParser elementParser, IBalloonChildNodeParser balloonTextChildNodeParser) 
            : base(elementParser, balloonTextChildNodeParser)
        {
        }
    }

    private readonly IElementParser elementParser;
    private readonly IBalloonChildNodeParser balloonTextChildNodeParser;
    private readonly TestClass sut;

    public BalloonTextNodeParserBaseTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        balloonTextChildNodeParser = Helper.FakeParser<IBalloonChildNodeParser>("abc");
        A.CallTo(() => balloonTextChildNodeParser.BalloonType).Returns(BalloonType.Speech);

        sut = new(elementParser, balloonTextChildNodeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("abc");
        
        sut.Settings.ChildParsers.Count.Should().Be(1);
        sut.Settings.ChildParsers["abc"].Should().BeSameAs(balloonTextChildNodeParser);
        sut.Settings.NoRepeatNode.Should().BeTrue();
        sut.Settings.ExitOnUnknownNode.Should().BeTrue();

        sut.AggregationSettings.Should().NotBeNull();
        sut.AggregationSettings.ChildParsers.Count.Should().Be(1);
        sut.AggregationSettings.ChildParsers["abc"].Should().BeSameAs(balloonTextChildNodeParser);
        sut.Settings.NoRepeatNode.Should().BeFalse();
        sut.Settings.ExitOnUnknownNode.Should().BeTrue();
    }

    [Fact]
    public async Task NoAggregation()
    {
        var parentContext = new FakeParentParsingContext();
        var context = A.Dummy<IParsingContext>();
        var reader = A.Dummy<XmlReader>();
        A.CallTo(() => reader.ReadState).Returns(ReadState.Interactive);

        var dummyNode1 = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(dummyNode1));

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        var node = parentContext.Nodes.Should().ContainSingle()
            .Which.Should().BeOfType<BalloonNode>().Which;

        node.BallonType.Should().Be(BalloonType.Speech);
        node.ChildBlock.ForwardQueue.Dequeue().Should().BeSameAs(dummyNode1);
        node.ChildBlock.ForwardQueue.Count.Should().Be(0);

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

        var dummyNode1 = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(dummyNode1));

        var dummyNode2 = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(dummyNode2));

        await sut.ParseAsync(reader, context, parentContext);

        var node = parentContext.Nodes.Should().ContainSingle()
            .Which.Should().BeOfType<BalloonNode>().Which;

        node.BallonType.Should().Be(BalloonType.Speech);
        node.ChildBlock.ForwardQueue.Dequeue().Should().BeSameAs(dummyNode1);
        node.ChildBlock.ForwardQueue.Dequeue().Should().BeSameAs(dummyNode2);
        node.ChildBlock.ForwardQueue.Count.Should().Be(0);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldNotTryAggregateAtEndOfElement()
    {
        var parentContext = new FakeParentParsingContext();
        var context = A.Dummy<IParsingContext>();
        var reader = A.Dummy<XmlReader>();
        A.CallTo(() => reader.ReadState).Returns(ReadState.EndOfFile);

        var dummyNode1 = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(dummyNode1));

        await sut.ParseAsync(reader, context, parentContext);

        var node = parentContext.Nodes.Should().ContainSingle()
            .Which.Should().BeOfType<BalloonNode>().Which;

        node.BallonType.Should().Be(BalloonType.Speech);
        node.ChildBlock.ForwardQueue.Dequeue().Should().BeSameAs(dummyNode1);
        node.ChildBlock.ForwardQueue.Count.Should().Be(0);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).MustHaveHappenedOnceExactly();
    }
}
