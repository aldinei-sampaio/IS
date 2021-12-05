using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class MusicNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly INameTextParser nameTextParser;
    private readonly MusicNodeParser sut;

    public MusicNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = Helper.FakeParser<IWhenAttributeParser>("when");        
        nameTextParser = A.Dummy<INameTextParser>();
        sut = new(elementParser, whenAttributeParser, nameTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("music");
        sut.Settings.TextParser.Should().BeSameAs(nameTextParser);
        sut.Settings.AttributeParsers["when"].Should().BeSameAs(whenAttributeParser);
        sut.Settings.AttributeParsers.Count.Should().Be(1);
        sut.Settings.ChildParsers.Count.Should().Be(0);
    }

    [Theory]
    [InlineData("open_sky", "open_sky")]
    [InlineData("", null)]
    public async Task ParseAsyncShouldReturnMusicNode(string parsedValue, string musicName)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var when = A.Dummy<ICondition>();
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = parsedValue;
        parsed.When = when;
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var ret = await sut.ParseAsync(reader, context);
        var protagonistNode = ret.Should().BeOfType<MusicNode>().Which;
        protagonistNode.MusicName.Should().Be(musicName);
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
    public void DismissNodeShouldClearMusic()
    {
        var node = sut.DismissNode;
        var dismiss = node.Should().BeOfType<DismissNode<MusicNode>>().Which;
        var protagNode = dismiss.ChangeNode.Should().BeOfType<MusicNode>().Which;
        protagNode.MusicName.Should().BeNull();
        protagNode.When.Should().BeNull();
    }
}
