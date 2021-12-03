using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class PersonNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly INameTextParser nameTextParser;
    private readonly ISpeechNodeParser speechNodeParser;
    private readonly IThoughtNodeParser thoughtNodeParser;
    private readonly IMoodNodeParser moodNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly PersonNodeParser sut;
    
    public PersonNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        nameTextParser = A.Dummy<INameTextParser>();
        speechNodeParser = Helper.FakeParser<ISpeechNodeParser>("speech");
        thoughtNodeParser = Helper.FakeParser<IThoughtNodeParser>("thought");
        moodNodeParser = Helper.FakeParser<IMoodNodeParser>("mood");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");
        sut = new(elementParser, nameTextParser, speechNodeParser, thoughtNodeParser, moodNodeParser, pauseNodeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("person");
        sut.Settings.TextParser.Should().BeSameAs(nameTextParser);
        sut.Settings.AttributeParsers.Count.Should().Be(0);
        sut.Settings.ChildParsers.Count.Should().Be(0);

        sut.Aggregation.Should().NotBeNull();
        sut.Aggregation.ChildParsers["speech"].Should().BeSameAs(speechNodeParser);
        sut.Aggregation.ChildParsers["thought"].Should().BeSameAs(thoughtNodeParser);
        sut.Aggregation.ChildParsers["mood"].Should().BeSameAs(moodNodeParser);
        sut.Aggregation.ChildParsers["pause"].Should().BeSameAs(pauseNodeParser);
        sut.Aggregation.ChildParsers.Count.Should().Be(4);
    }

    [Fact]
    public async Task ParseAsyncShouldReturnAPersonNode()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = "lorenipsum";
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var ret = await sut.ParseAsync(reader, context);

        var personNode = ret.Should().BeOfType<PersonNode>().Which;
        personNode.PersonName.Should().Be("lorenipsum");
        personNode.ChildBlock.Should().NotBeNull();
        personNode.ChildBlock.ForwardQueue.Count.Should().Be(0);

        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParseAsyncShouldReturnNullIfParsedTextIsNull()
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
    public async Task ParseAsyncShouldLogErrorIfParsedTextIsEmpty()
    {
        var errorMessage = "Era esperado o nome do personagem.";
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = string.Empty;
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);
        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();

        var ret = await sut.ParseAsync(reader, context);
        ret.Should().BeNull();

        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void AggregateShouldReturnNullWhenBlockIsEmpty()
    {
        var block = A.Dummy<IBlock>();
        var ret = sut.Aggregate(block);
        ret.Should().BeNull();
    }

    [Fact]
    public void AggregateShouldReturnNullIfFirstNodeIsNotPersonNode()
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
        var node = new PersonNode("omega", null);
        block.ForwardQueue.Enqueue(node);
        var ret = sut.Aggregate(block);
        ret.Should().BeNull();
    }

    [Fact]
    public void AggregateChildNodeOfPersonNodeShouldNotBeNull()
    {
        var block = A.Dummy<IBlock>();
        var node = new PersonNode("jirama", null);
        block.ForwardQueue.Enqueue(node);
        block.ForwardQueue.Enqueue(A.Dummy<INode>());
        Assert.Throws<InvalidOperationException>(() => sut.Aggregate(block));
    }

    [Fact]
    public void AggregateShouldAppendAllOtherNodesAsChildrenOfThePersonNode()
    {
        var personNode = new PersonNode("navira", A.Dummy<IBlock>());
        var node1 = A.Dummy<INode>();
        var node2 = A.Dummy<INode>();
        var node3 = A.Dummy<INode>();

        var block = A.Dummy<IBlock>();
        block.ForwardQueue.Enqueue(personNode);
        block.ForwardQueue.Enqueue(node1);
        block.ForwardQueue.Enqueue(node2);
        block.ForwardQueue.Enqueue(node3);

        var ret = sut.Aggregate(block);

        ret.Should().BeSameAs(personNode);
        personNode.ChildBlock.ForwardQueue.Count.Should().Be(3);
        personNode.ChildBlock.ForwardQueue.Dequeue().Should().BeSameAs(node1);
        personNode.ChildBlock.ForwardQueue.Dequeue().Should().BeSameAs(node2);
        personNode.ChildBlock.ForwardQueue.Dequeue().Should().BeSameAs(node3);
    }
}
