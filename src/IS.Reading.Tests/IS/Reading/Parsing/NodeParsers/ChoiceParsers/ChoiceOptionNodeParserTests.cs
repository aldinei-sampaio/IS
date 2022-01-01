using IS.Reading.Choices;
using IS.Reading.Conditions;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionNodeParserTests
{
    private readonly ChoiceOptionNodeParser sut;
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly IBalloonTextParser balloonTextParser;
    private readonly IChoiceOptionTextNodeParser choiceOptionTextNodeParser;
    private readonly IChoiceOptionEnabledWhenNodeParser choiceOptionEnabledWhenNodeParser;
    private readonly IChoiceOptionDisabledTextNodeParser choiceOptionDisabledTextNodeParser;
    private readonly IChoiceOptionIconNodeParser choiceOptionIconNodeParser;

    public ChoiceOptionNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = A.Dummy<IWhenAttributeParser>();
        balloonTextParser = A.Dummy<IBalloonTextParser>();
        choiceOptionTextNodeParser = Helper.FakeParser<IChoiceOptionTextNodeParser>("a");
        choiceOptionEnabledWhenNodeParser = Helper.FakeParser<IChoiceOptionEnabledWhenNodeParser>("enabledwhen");
        choiceOptionDisabledTextNodeParser = Helper.FakeParser<IChoiceOptionDisabledTextNodeParser>("disabledtext");
        choiceOptionIconNodeParser = Helper.FakeParser<IChoiceOptionIconNodeParser>("icon");
        sut = new(
            elementParser, 
            whenAttributeParser, 
            balloonTextParser, 
            choiceOptionTextNodeParser,
            choiceOptionEnabledWhenNodeParser,
            choiceOptionDisabledTextNodeParser,
            choiceOptionIconNodeParser
        );
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().BeEmpty();
        sut.NameRegex.Should().Be("^[a-z]$");
        sut.Settings.ShouldBeNonRepeat(
            whenAttributeParser, 
            balloonTextParser,
            choiceOptionTextNodeParser,
            choiceOptionEnabledWhenNodeParser,
            choiceOptionDisabledTextNodeParser,
            choiceOptionIconNodeParser
        );
    }

    [Fact]
    public async Task ShoudCreateOptionInParentContext()
    {
        var reader = A.Dummy<XmlReader>();
        A.CallTo(() => reader.LocalName).Returns("n");

        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new ChoiceParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<ChoiceOptionParentParsingContext>(2).Option.Text = "Loren Ipsum");

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.Choice.Options.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new
            {
                Key = "n",
                Text = "Loren Ipsum",
                DisabledText = (string)null,
                EnabledWhen = (ICondition)null,
                VisibleWhen = (ICondition)null
            });
    }

    [Fact]
    public async Task ShouldCopyOptionalPropertiesToOptionNode()
    {
        var reader = A.Dummy<XmlReader>();
        A.CallTo(() => reader.LocalName).Returns("j");

        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new ChoiceParentParsingContext();

        var enabledWhen = A.Dummy<ICondition>();
        var visibleWhen = A.Dummy<ICondition>();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var ctx = i.GetArgument<ChoiceOptionParentParsingContext>(2);
                ctx.Option.Text = "Alpha Beta Gamma";
                ctx.Option.DisabledText = "Disabled";
                ctx.Option.EnabledWhen = enabledWhen;
                ctx.Option.VisibleWhen = visibleWhen;
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.Choice.Options.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new
            {
                Key = "j",
                Text = "Alpha Beta Gamma",
                DisabledText = "Disabled",
                EnabledWhen = enabledWhen,
                VisibleWhen = visibleWhen
            });
    }

    [Fact]
    public async Task ShouldLogErrorWhenOptionTextIsNotDefined()
    {
        var errorMessage = "O texto da opção não foi informado.";

        var reader = A.Dummy<XmlReader>();
        A.CallTo(() => reader.LocalName).Returns("x");

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();

        var parentContext = new ChoiceParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.Choice.Options.Should().BeEmpty();

        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
    }
}
