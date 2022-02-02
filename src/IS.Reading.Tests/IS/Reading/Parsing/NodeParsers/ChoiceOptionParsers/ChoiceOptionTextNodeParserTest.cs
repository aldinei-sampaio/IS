using IS.Reading.Choices;
using IS.Reading.Parsing.ArgumentParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public class ChoiceOptionTextNodeParserTest
{
    private readonly ChoiceOptionTextNodeParser sut;
    private readonly IElementParser elementParser;
    private readonly IBalloonTextParser balloonTextParser;

    public ChoiceOptionTextNodeParserTest()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        balloonTextParser = A.Dummy<IBalloonTextParser>();
        sut = new(elementParser, balloonTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("text");
        sut.Settings.ShouldBeNormal(balloonTextParser);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("Texto de botão de opção para o usuário clicar")]
    public async Task ShouldUpdateParentOptionText(string parsedText)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());

        var optionNode = A.Fake<IChoiceOptionNodeSetter>(i => i.Strict());
        A.CallToSet(() => optionNode.Text).To(parsedText).DoesNothing();

        var parentContext = A.Fake<IChoiceOptionParentParsingContext>(i => i.Strict());
        A.CallTo(() => parentContext.Option).Returns(optionNode);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = parsedText);

        await sut.ParseAsync(reader, context, parentContext);

        A.CallToSet(() => optionNode.Text).To(parsedText).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldDoNothingWhenParsedTextIsNull()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());

        var optionNode = A.Fake<IChoiceOptionNodeSetter>(i => i.Strict());

        var parentContext = A.Fake<IChoiceOptionParentParsingContext>(i => i.Strict());
        A.CallTo(() => parentContext.Option).Returns(optionNode);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = null);

        await sut.ParseAsync(reader, context, parentContext);
    }
}
