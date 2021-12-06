using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class MoodNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IMoodTextParser moodTextParser;
    private readonly ISpeechNodeParser speechNodeParser;
    private readonly IThoughtNodeParser thoughtNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly MoodNodeParser sut;

    public MoodNodeParserTests()
    {
        elementParser = A.Dummy<IElementParser>();
        moodTextParser = A.Dummy<IMoodTextParser>();
        speechNodeParser = Helper.FakeParser<ISpeechNodeParser>("speech");
        thoughtNodeParser = Helper.FakeParser<IThoughtNodeParser>("thought");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");

        sut = new MoodNodeParser(elementParser, moodTextParser, speechNodeParser, thoughtNodeParser, pauseNodeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("mood");

        sut.Settings.TextParser.Should().BeSameAs(moodTextParser);
        sut.Settings.AttributeParsers.Count.Should().Be(0);
        sut.Settings.ChildParsers.Count.Should().Be(0);

        sut.Aggregation.Should().NotBeNull();
        var childParsers = sut.Aggregation.ChildParsers;
        childParsers["speech"].Should().BeSameAs(speechNodeParser);
        childParsers["thought"].Should().BeSameAs(thoughtNodeParser);
        childParsers["pause"].Should().BeSameAs(pauseNodeParser);
        childParsers.Count.Should().Be(3);
    }

    [Fact]
    public async Task ParseAsyncShouldReturnMoodNode()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();

        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = "Happy";

        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var ret = await sut.ParseAsync(reader, context);
        var moodNode = ret.Should().BeOfType<MoodNode>().Which;
        moodNode.MoodType.Should().Be(MoodType.Happy);
        moodNode.ChildBlock.Should().NotBeNull();
        moodNode.ChildBlock.ForwardQueue.Count.Should().Be(0);
    }

    [Fact]
    public async Task ParseAsyncShouldReturnNullWhenParserReturnsNullText()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();

        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = null;

        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var ret = await sut.ParseAsync(reader, context);
        ret.Should().BeNull();
    }

    [Fact]
    public void AggregateShouldReturnNullWhenBlockIsEmpty()
    {
        var block = A.Dummy<IBlock>();
        var ret = sut.Aggregate(block);
        ret.Should().BeNull();
    }

    [Fact]
    public void AggregateShouldReturnNullIfFirstNodeIsNotMoodNode()
    {
        var block = A.Dummy<IBlock>();
        block.ForwardQueue.Enqueue(A.Dummy<INode>());
        block.ForwardQueue.Enqueue(A.Dummy<INode>());
        var ret = sut.Aggregate(block);
        ret.Should().BeNull();
    }

    [Fact]
    public void AggregateShouldReturnNullIfBlockHasLessThanTwoNodes()
    {
        var block = A.Dummy<IBlock>();
        var moodNode = new MoodNode(MoodType.Surprised, null);
        block.ForwardQueue.Enqueue(moodNode);
        var ret = sut.Aggregate(block);
        ret.Should().BeNull();
    }

    [Fact]
    public void AggregateChildNodeOfMoodNodeShouldNotBeNull()
    {
        var block = A.Dummy<IBlock>();
        var moodNode = new MoodNode(MoodType.Surprised, null);
        block.ForwardQueue.Enqueue(moodNode);
        block.ForwardQueue.Enqueue(A.Dummy<INode>());
        Assert.Throws<InvalidOperationException>(() => sut.Aggregate(block));
    }

    [Fact]
    public void AggregateShouldAppendAllOtherNodesAsChildrenOfTheMoodNode()
    {
        var moodNode = new MoodNode(MoodType.Surprised, A.Dummy<IBlock>());
        var node1 = A.Dummy<INode>();
        var node2 = A.Dummy<INode>();
        var node3 = A.Dummy<INode>();

        var block = A.Dummy<IBlock>();
        block.ForwardQueue.Enqueue(moodNode);
        block.ForwardQueue.Enqueue(node1);
        block.ForwardQueue.Enqueue(node2);
        block.ForwardQueue.Enqueue(node3);

        var ret = sut.Aggregate(block);

        ret.Should().BeSameAs(moodNode);
        moodNode.ChildBlock.ForwardQueue.Count.Should().Be(3);
        moodNode.ChildBlock.ForwardQueue.Dequeue().Should().BeSameAs(node1);
        moodNode.ChildBlock.ForwardQueue.Dequeue().Should().BeSameAs(node2);
        moodNode.ChildBlock.ForwardQueue.Dequeue().Should().BeSameAs(node3);
    }
}
