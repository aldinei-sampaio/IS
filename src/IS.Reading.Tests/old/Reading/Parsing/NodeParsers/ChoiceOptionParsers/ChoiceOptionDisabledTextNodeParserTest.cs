using IS.Reading.Parsing.ArgumentParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public class ChoiceOptionDisabledTextNodeParserTest
{
    private readonly ChoiceOptionDisabledTextNodeParser sut;
    private readonly IElementParser elementParser;
    private readonly IBalloonTextParser balloonTextParser;

    public ChoiceOptionDisabledTextNodeParserTest()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        balloonTextParser = A.Dummy<IBalloonTextParser>();
        sut = new(elementParser, balloonTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("disabledtext");
        sut.Settings.ShouldBeNormal(balloonTextParser);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Let's do it!")]
    public async Task ShouldUpdateParentContextDisabledText(string parsedText)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new ChoiceOptionParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = parsedText);

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.Option.DisabledText.Should().Be(parsedText);
    }
}
