using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class BalloonChildNodeParserBaseTests
{
    private class TestClass : BalloonChildNodeParserBase
    {
        public TestClass(IElementParser elementParser, IBalloonTextNodeParser balloonTextNodeParser, IChoiceNodeParser choiceNodeParser) 
            : base(elementParser, balloonTextNodeParser, choiceNodeParser)
        {
        }
    }

    private readonly IElementParser elementParser;
    private readonly IBalloonTextNodeParser balloonTextNodeParser;
    private readonly IChoiceNodeParser choiceNodeParser;
    private readonly TestClass sut;

    public BalloonChildNodeParserBaseTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        balloonTextNodeParser = Helper.FakeParser<IBalloonTextNodeParser>("xyz");
        A.CallTo(() => balloonTextNodeParser.BalloonType).Returns(BalloonType.Narration);
        choiceNodeParser = Helper.FakeParser<IChoiceNodeParser>("choice");
        sut = new (elementParser, balloonTextNodeParser, choiceNodeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("xyz");
        sut.BalloonType.Should().Be(BalloonType.Narration);
        sut.Settings.ShouldBeNoRepeat(balloonTextNodeParser);
        sut.AggregationSettings.ShouldBeAggregate(choiceNodeParser);
    }

    [Fact]
    public async Task ShouldReturnNodeWhenParsedTextIsNotNull()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = "abc");

        await sut.ParseAsync(reader, context, parentContext);

        var parsed = parentContext.Nodes.Should().ContainSingle()
            .Which.Should().BeOfType<BalloonTextNode>().Which;

        parsed.Text.Should().Be("abc");
        parsed.BalloonType.Should().Be(BalloonType.Speech);
    }

    [Fact]
    public async Task ShouldReturnNullWhenParsedTextIsNull()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);
        parentContext.ShouldBeEmpty();
    }
}
