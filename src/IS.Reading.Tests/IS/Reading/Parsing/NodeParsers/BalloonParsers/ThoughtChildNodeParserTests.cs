namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class ThoughtChildNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var thoughtTextNodeParser = Helper.FakeParser<IThoughtTextNodeParser>("thought");
        A.CallTo(() => thoughtTextNodeParser.BalloonType).Returns(BalloonType.Thought);
        var choiceNodeParser = Helper.FakeParser<IChoiceNodeParser>("choice");

        var sut = new ThoughtChildNodeParser(elementParser, thoughtTextNodeParser, choiceNodeParser);
        sut.Name.Should().Be("thought");
        sut.BalloonType.Should().Be(BalloonType.Thought);
        sut.Settings.ShouldBeAggregatedNonRepeat(thoughtTextNodeParser);
        sut.AggregationSettings.ShouldBeAggregatedNonRepeat(choiceNodeParser);
    }
}
