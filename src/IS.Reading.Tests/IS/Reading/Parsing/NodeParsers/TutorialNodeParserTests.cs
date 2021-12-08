using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class TutorialNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var tutorialChildNodeParser = Helper.FakeParser<ITutorialChildNodeParser>("tomorrow");
        var sut = new TutorialNodeParser(elementParser, tutorialChildNodeParser);
        sut.Name.Should().Be("tomorrow");
        sut.Settings.ShouldBeAggregatedNonRepeat(tutorialChildNodeParser);
        sut.AggregationSettings.ShouldBeAggregated(tutorialChildNodeParser);
    }
}
