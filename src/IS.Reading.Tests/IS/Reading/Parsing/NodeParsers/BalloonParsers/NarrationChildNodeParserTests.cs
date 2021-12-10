namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class NarrationChildNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var narrationTextNodeParser = Helper.FakeParser<INarrationTextNodeParser>("narration");
        A.CallTo(() => narrationTextNodeParser.BalloonType).Returns(BalloonType.Narration);
        var choiceNodeParser = Helper.FakeParser<IChoiceNodeParser>("choice");

        var sut = new NarrationChildNodeParser(elementParser, narrationTextNodeParser, choiceNodeParser);
        sut.Name.Should().Be("narration");
        sut.BalloonType.Should().Be(BalloonType.Narration);
        sut.Settings.ShouldBeAggregatedNonRepeat(narrationTextNodeParser);
        sut.AggregationSettings.ShouldBeAggregatedNonRepeat(choiceNodeParser);
    }
}
