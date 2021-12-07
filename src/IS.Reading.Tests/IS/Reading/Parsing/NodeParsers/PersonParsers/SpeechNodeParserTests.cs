using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class SpeechNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var speechChildNodeParser = Helper.FakeParser<ISpeechChildNodeParser>("bizantino");
        var sut = new SpeechNodeParser(elementParser, speechChildNodeParser);
        sut.Name.Should().Be("bizantino");
        sut.Settings.ShouldBeNoRepeat(speechChildNodeParser);
        sut.AggregationSettings.ShouldBeAggregate(speechChildNodeParser);
    }
}
