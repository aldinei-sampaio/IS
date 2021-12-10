using IS.Reading.Choices;
using IS.Reading.Parsing.NodeParsers.ChoiceParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class ChoiceNodeParserTests
{
    private readonly ChoiceNodeParser sut;
    private readonly IElementParser elementParser;
    private readonly IChoiceTimeLimitNodeParser timeLimitNodeParser;
    private readonly IChoiceDefaultNodeParser defaultNodeParser;
    private readonly IChoiceOptionNodeParser optionNodeParser;

    public ChoiceNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        timeLimitNodeParser = Helper.FakeParser<IChoiceTimeLimitNodeParser>("alpha");
        defaultNodeParser = Helper.FakeParser<IChoiceDefaultNodeParser>("beta");
        optionNodeParser = Helper.FakeParser<IChoiceOptionNodeParser>("gamma");

        sut = new(elementParser, timeLimitNodeParser, defaultNodeParser, optionNodeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("choice");
        sut.Settings.ShouldBeNormal(timeLimitNodeParser, defaultNodeParser, optionNodeParser);
    }

    [Fact]
    public async Task OptionalPropertiesNonInformed()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new BalloonChildParsingContext();

        var optionNode1 = A.Dummy<IChoiceOptionNode>();
        var optionNode2 = A.Dummy<IChoiceOptionNode>();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var ctx = i.GetArgument<ChoiceParentParsingContext>(2);
                ctx.Choice.Options.Add(optionNode1);
                ctx.Choice.Options.Add(optionNode2);
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ChoiceNode.Should().NotBeNull();
        parentContext.ChoiceNode.TimeLimit.Should().BeNull();
        parentContext.ChoiceNode.Default.Should().BeNull();
        parentContext.ChoiceNode.RandomOrder.Should().BeFalse();
        parentContext.ChoiceNode.Options.Count.Should().Be(2);
        parentContext.ChoiceNode.Options[0].Should().BeSameAs(optionNode1);
        parentContext.ChoiceNode.Options[1].Should().BeSameAs(optionNode2);
    }

    [Fact]
    public async Task OptionalPropertiesInformed()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new BalloonChildParsingContext();

        var optionNode1 = A.Dummy<IChoiceOptionNode>();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var ctx = i.GetArgument<ChoiceParentParsingContext>(2);
                ctx.Choice.Options.Add(optionNode1);
                ctx.Choice.TimeLimit = TimeSpan.FromSeconds(8);
                ctx.Choice.Default = "a";
                ctx.Choice.RandomOrder = true;
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ChoiceNode.Should().NotBeNull();
        parentContext.ChoiceNode.TimeLimit.Should().Be(TimeSpan.FromSeconds(8));
        parentContext.ChoiceNode.Default.Should().Be("a");
        parentContext.ChoiceNode.RandomOrder.Should().BeTrue();
        parentContext.ChoiceNode.Options.Count.Should().Be(1);
        parentContext.ChoiceNode.Options[0].Should().BeSameAs(optionNode1);
    }

    [Fact]
    public async Task ShouldLogErrorWhenNoOptionWasParsed()
    {
        var message = "Nenhuma opção informada.";

        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var parentContext = new BalloonChildParsingContext();

        var optionNode1 = A.Dummy<IChoiceOptionNode>();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ChoiceNode.Should().BeNull();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }
}
