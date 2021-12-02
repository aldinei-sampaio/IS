using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.Helpers;

public class BalloonTextNodeParserBaseTests
{
    private class TestClass : BalloonTextNodeParserBase
    {
        public TestClass(string name, BalloonType balloonType, IElementParser elementParser, IBalloonTextParser balloonTextParser) 
            : base(name, balloonType, elementParser, balloonTextParser)
        {
        }

        public INodeParser FakeChildParser()
        {
            ChildParser = A.Fake<INodeParser>();
            return ChildParser;
        }
    }

    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextparser = A.Dummy<IBalloonTextParser>();
        var sut = new TestClass("chorume", BalloonType.Thought, elementParser, balloonTextparser);
        sut.Name.Should().Be("chorume");
        
        var childParser = sut.ChildParser.Should().BeOfType<BalloonTextChildNodeParser>().Which;
        childParser.Name.Should().Be("chorume");
        childParser.BalloonType.Should().Be(BalloonType.Thought);
        childParser.Settings.TextParser.Should().BeSameAs(balloonTextparser);
        childParser.Settings.AttributeParsers.Count.Should().Be(0);
        childParser.Settings.ChildParsers.Count.Should().Be(0);

        sut.Aggregation.Should().NotBeNull();
        sut.Aggregation.ChildParsers.Count.Should().Be(1);
        sut.Aggregation.ChildParsers["chorume"].Should().BeSameAs(childParser);
    }

    [Fact]
    public async Task ParseAsyncShouldCallChildParser()
    {
        var context = A.Dummy<IParsingContext>();
        var reader = A.Dummy<XmlReader>();

        var elementParser = A.Dummy<IElementParser>();
        var balloonTextparser = A.Dummy<IBalloonTextParser>();
        var sut = new TestClass("teste", BalloonType.Speech, elementParser, balloonTextparser);

        var childParser = sut.FakeChildParser();
        var node = A.Fake<INode>();
        A.CallTo(() => childParser.ParseAsync(reader, context)).Returns(node);

        var ret = await sut.ParseAsync(reader, context);
        ret.Should().Be(node);

        A.CallTo(() => childParser.ParseAsync(reader, context)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void AggregateShouldCreateBalloonNode()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextparser = A.Dummy<IBalloonTextParser>();
        var sut = new TestClass("teste", BalloonType.Thought, elementParser, balloonTextparser);

        var block = A.Dummy<IBlock>();

        var ret = sut.Aggregate(block);
        var node = ret.Should().BeOfType<BalloonNode>().Which;
        node.ChildBlock.Should().BeSameAs(block);
        node.BallonType.Should().Be(BalloonType.Thought);
    }
}
