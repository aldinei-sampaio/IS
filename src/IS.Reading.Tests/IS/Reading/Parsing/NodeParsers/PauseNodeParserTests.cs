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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task NormalPause(string text)
    {
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = text;
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().NotBeNull();
        result.Should().BeOfType<PauseNode>()
            .Which.When.Should().BeSameAs(parsed.When);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(19)]
    [InlineData(258)]
    [InlineData(1234)]
    [InlineData(5000)]
    public async Task TimedPause(int value)
    {
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = value.ToString();
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().NotBeNull();
        var node = result.Should().BeOfType<TimedPauseNode>().Which;
        node.When.Should().BeSameAs(parsed.When);
        node.Duration.Should().Be(TimeSpan.FromMilliseconds(value));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-5000")]
    public async Task DurationShouldBeGreaterThanZero(string duration)
    {
        const string message = "O tempo de espera precisa ser maior que zero.";
        await CheckForValidationErrorAsync(duration, message);
    }

    [Theory]
    [InlineData("5001")]
    [InlineData("6000")]
    [InlineData("10000")]
    [InlineData("999999999")]
    public async Task DurationShouldBeUpTo5000(string duration)
    {
        const string message = "O tempo de espera não pode ser maior que 5000.";
        await CheckForValidationErrorAsync(duration, message);
    }

    private async Task CheckForValidationErrorAsync(string duration, string message)
    {
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = duration;
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);
        result.Should().BeNull();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }
}
