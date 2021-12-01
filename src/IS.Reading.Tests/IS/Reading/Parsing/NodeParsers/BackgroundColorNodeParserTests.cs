using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.State;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class BackgroundColorNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly IColorTextParser colorTextParser;
    private readonly BackgroundColorNodeParser sut;

    public BackgroundColorNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = Helper.FakeParser<IWhenAttributeParser>("when");
        colorTextParser = A.Dummy<IColorTextParser>();

        sut = new(elementParser, whenAttributeParser, colorTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("color");
        sut.Settings.AttributeParsers.Count.Should().Be(1);
        sut.Settings.AttributeParsers["when"].Should().BeSameAs(whenAttributeParser);
        sut.Settings.ChildParsers.Count.Should().Be(0);
        sut.Settings.TextParser.Should().BeSameAs(colorTextParser);
    }

    [Fact]
    public async Task Success()
    {
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = "alfa";
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().NotBeNull();
        result.Should().BeOfType<BackgroundNode>();

        var node = (BackgroundNode)result;
        node.When.Should().BeSameAs(parsed.When);
        node.State.Name.Should().Be(parsed.Text);
        node.State.Type.Should().Be(BackgroundType.Color);
        node.State.Position.Should().Be(BackgroundPosition.Undefined);
    }

    [Fact]
    public async Task EmptyColorName()
    {
        const string message = "Era esperado o nome da cor.";
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = string.Empty;
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);
        result.Should().BeNull();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task InvalidColorName()
    {
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = null; // O ColorTextParser irá retornar NULL se o nome da cor for inválido
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);
        result.Should().BeNull();
    }
}
