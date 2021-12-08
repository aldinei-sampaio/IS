using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class NarrationNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var narrationChildNodeParser = Helper.FakeParser<INarrationChildNodeParser>("forget");
        var sut = new NarrationNodeParser(elementParser, narrationChildNodeParser);
        sut.Name.Should().Be("forget");
        sut.Settings.ShouldBeAggregatedNonRepeat(narrationChildNodeParser);
        sut.AggregationSettings.ShouldBeAggregated(narrationChildNodeParser);
    }
}
