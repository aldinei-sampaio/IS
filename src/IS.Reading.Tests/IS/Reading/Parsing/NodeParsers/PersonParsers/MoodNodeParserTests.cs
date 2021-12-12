using IS.Reading.Navigation;
using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class MoodNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IMoodTextNodeParser moodTextNodeParser;
    private readonly ISpeechNodeParser speechNodeParser;
    private readonly IThoughtNodeParser thoughtNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly MoodNodeParser sut;

    public MoodNodeParserTests()
    {
        elementParser = A.Dummy<IElementParser>();
        moodTextNodeParser = Helper.FakeParser<IMoodTextNodeParser>("mood");
        speechNodeParser = Helper.FakeParser<ISpeechNodeParser>("speech");
        thoughtNodeParser = Helper.FakeParser<IThoughtNodeParser>("thought");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");

        sut = new MoodNodeParser(elementParser, moodTextNodeParser, speechNodeParser, thoughtNodeParser, pauseNodeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("mood");
        sut.Settings.ShouldBeAggregatedNonRepeat(moodTextNodeParser);
        sut.AggregationSettings.ShouldBeAggregated(speechNodeParser, thoughtNodeParser, pauseNodeParser);
    }

    [Fact]
    public async Task ParseAsyncShouldReturnMoodNode()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = "Happy");

        var dummyNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).AddNode(dummyNode));

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<MoodNode>(i =>
        {
            i.MoodType.Should().Be(MoodType.Happy);
            i.ChildBlock.ShouldContainOnly(dummyNode);
        });
    }

    [Fact]
    public async Task ParseAsyncShouldReturnNullWhenParserReturnsNullText()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);
        parentContext.ShouldBeEmpty();
    }
}
