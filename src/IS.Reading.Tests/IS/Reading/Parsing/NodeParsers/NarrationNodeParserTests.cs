using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class NarrationNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var narrationChildNodeParser = Helper.FakeParser<INarrationChildNodeParser>("forget");
        var setNodeParser = Helper.FakeParser<ISetNodeParser>("set");
        var unsetNodeParser = Helper.FakeParser<IUnsetNodeParser>("unset");
        var sut = new NarrationNodeParser(elementParser, narrationChildNodeParser, setNodeParser, unsetNodeParser);
        sut.Name.Should().Be("forget");
        sut.Settings.ShouldBeAggregatedNonRepeat(narrationChildNodeParser);
        sut.AggregationSettings.ShouldBeAggregated(narrationChildNodeParser, setNodeParser, unsetNodeParser);
    }
}
