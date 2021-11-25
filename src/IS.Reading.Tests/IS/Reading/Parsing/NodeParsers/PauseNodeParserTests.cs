using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class PauseNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly IIntegerTextParser integerTextParser;
    private readonly PauseNodeParser sut;

    public PauseNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = A.Dummy<IWhenAttributeParser>();
        integerTextParser = A.Fake<IIntegerTextParser>();

        sut = new(elementParser, whenAttributeParser, integerTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.ElementName.Should().Be("pause");
        sut.Settings.AttributeParsers.Should().ContainValues(whenAttributeParser);
        sut.Settings.ChildParsers.Should().BeEmpty();
        sut.Settings.TextParser.Should().BeSameAs(integerTextParser);
    }

    [Fact]
    public async Task Success()
    {
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = null;
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().NotBeNull();
        var node = result.Should().BeOfType<PauseNode>().Which;
        node.When.Should().BeSameAs(parsed.When);
        node.Duration.Should().BeNull();
    }

    [Fact]
    public async Task Duration()
    {
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = "1000";
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().NotBeNull();
        var node = result.Should().BeOfType<PauseNode>().Which;
        node.When.Should().BeSameAs(parsed.When);
        node.Duration.Should().Be(TimeSpan.FromSeconds(1));
    }

}
