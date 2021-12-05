using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class ProtagonistNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly INameTextParser nameTextParser;
    private readonly ProtagonistNodeParser sut;

    public ProtagonistNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = Helper.FakeParser<IWhenAttributeParser>("when");        
        nameTextParser = A.Dummy<INameTextParser>();
        sut = new(elementParser, whenAttributeParser, nameTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("protagonist");
        sut.Settings.TextParser.Should().BeSameAs(nameTextParser);
        sut.Settings.AttributeParsers["when"].Should().BeSameAs(whenAttributeParser);
        sut.Settings.AttributeParsers.Count.Should().Be(1);
        sut.Settings.ChildParsers.Count.Should().Be(0);
    }

    [Theory]
    [InlineData("chamusca", "chamusca")]
    [InlineData("", null)]
    public async Task ParseAsyncShouldReturnProtagonistNode(string parsedValue, string protagonistName)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var when = A.Dummy<ICondition>();
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = parsedValue;
        parsed.When = when;
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var ret = await sut.ParseAsync(reader, context);
        var protagonistNode = ret.Should().BeOfType<ProtagonistNode>().Which;
        protagonistNode.ProtagonistName.Should().Be(protagonistName);
        protagonistNode.When.Should().BeSameAs(when);

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
    public void DismissNodeShouldClearProtagonist()
    {
        var node = sut.DismissNode;
        var dismiss = node.Should().BeOfType<DismissNode<ProtagonistNode>>().Which;
        var protagNode = dismiss.ChangeNode.Should().BeOfType<ProtagonistNode>().Which;
        protagNode.ProtagonistName.Should().BeNull();
        protagNode.When.Should().BeNull();
    }
}
