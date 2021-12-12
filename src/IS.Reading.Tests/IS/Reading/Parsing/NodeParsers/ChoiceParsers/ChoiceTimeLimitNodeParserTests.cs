using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceTimeLimitNodeParserTests
{
    private readonly ChoiceTimeLimitNodeParser sut;
    private readonly IElementParser elementParser;
    private readonly IIntegerTextParser integerTextParser;

    public ChoiceTimeLimitNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        integerTextParser = A.Dummy<IIntegerTextParser>();
        sut = new(elementParser, integerTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("timelimit");
        sut.Settings.ShouldBeNormal(integerTextParser);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("5000")]
    [InlineData("30000")]
    public async Task ShouldSetParentContextTimeLimit(string parsedText)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new ChoiceParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = parsedText);

        await sut.ParseAsync(reader, context, parentContext);

        var expected = TimeSpan.FromMilliseconds(int.Parse(parsedText));
        parentContext.Choice.TimeLimit.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("0")]
    public async Task ShouldIgnoreZeroOrEmptyText(string parsedText)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new ChoiceParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = parsedText);

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.Choice.TimeLimit.Should().BeNull();
    }

    [Fact]
    public async Task ShouldLogErrorWhenParsedTimeIsBiggerThan30000()
    {
        const string message = "O limite de tempo não pode ser maior que 30 segundos.";

        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var parentContext = new ChoiceParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = "30001");

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.Choice.TimeLimit.Should().BeNull();
        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }
}
