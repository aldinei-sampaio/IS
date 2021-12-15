using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class SpeechNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var speechChildNodeParser = Helper.FakeParser<ISpeechChildNodeParser>("bizantino");
        var moodNodeParser = A.Dummy<IMoodNodeParser>();
        var sut = new SpeechNodeParser(elementParser, speechChildNodeParser, moodNodeParser);
        sut.Name.Should().Be("bizantino");
        sut.Settings.ShouldBeAggregatedNonRepeat(speechChildNodeParser);
        sut.AggregationSettings.ShouldBeAggregated(speechChildNodeParser, moodNodeParser);
    }
}
