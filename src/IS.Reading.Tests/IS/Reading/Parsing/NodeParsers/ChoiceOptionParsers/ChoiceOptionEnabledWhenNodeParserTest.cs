using IS.Reading.Choices;
using IS.Reading.Navigation;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public class ChoiceOptionEnabledWhenNodeParserTest
{
    private readonly ChoiceOptionEnabledWhenNodeParser sut;
    private readonly IElementParser elementParser;
    private readonly IBalloonTextParser balloonTextParser;
    private readonly IConditionParser conditionParser;

    public ChoiceOptionEnabledWhenNodeParserTest()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        balloonTextParser = A.Dummy<IBalloonTextParser>();
        conditionParser = A.Fake<IConditionParser>(i => i.Strict());
        sut = new(elementParser, balloonTextParser, conditionParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("enabledwhen");
        sut.Settings.ShouldBeNormal(balloonTextParser);
    }

    [Fact]
    public async Task ShouldUpdateParentContextEnabledWhen()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());

        var condition = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => conditionParser.Parse("condição")).Returns(condition);

        var optionNode = A.Fake<IChoiceOptionNodeSetter>(i => i.Strict());
        A.CallToSet(() => optionNode.EnabledWhen).To(condition).DoesNothing();

        var parentContext = A.Fake<IChoiceOptionParentParsingContext>(i => i.Strict());
        A.CallTo(() => parentContext.Option).Returns(optionNode);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = "condição");

        await sut.ParseAsync(reader, context, parentContext);

        A.CallToSet(() => optionNode.EnabledWhen).To(condition).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldDoNothingWhenParsedTextIsNull()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = A.Fake<IChoiceOptionParentParsingContext>(i => i.Strict());

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = null);

        await sut.ParseAsync(reader, context, parentContext);
    }

    [Fact]
    public async Task ShouldLogErrorWhenParsedTextIsNotAValidCondition()
    {
        const string message = "Condição inválida.";

        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var parentContext = A.Fake<IChoiceOptionParentParsingContext>(i => i.Strict());

        var condition = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => conditionParser.Parse("gibberish")).Returns(null);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = "gibberish");

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }
}
