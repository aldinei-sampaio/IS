using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class PauseNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly PauseNodeParser sut;

    public PauseNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = A.Dummy<IWhenAttributeParser>();

        sut = new(elementParser, whenAttributeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.ElementName.Should().Be("pause");
        sut.Settings.AttributeParsers.Should().ContainValues(whenAttributeParser);
        sut.Settings.ChildParsers.Should().BeEmpty();
        sut.Settings.TextParser.Should().BeNull();
    }

    [Fact]
    public async Task Success()
    {
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = null;
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().NotBeNull();
        result.Should().BeOfType<PauseNode>();

        var node = (PauseNode)result;
        node.When.Should().BeSameAs(parsed.When);
        node.While.Should().BeNull();
        node.ChildBlock.Should().BeNull();
    }
}
