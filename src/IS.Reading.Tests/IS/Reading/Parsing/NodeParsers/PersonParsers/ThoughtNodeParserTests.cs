using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class ThoughtNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var thoughtChildNodeParser = Helper.FakeParser<IThoughtChildNodeParser>("oceania");
        var sut = new ThoughtNodeParser(elementParser, thoughtChildNodeParser);
        sut.Name.Should().Be("oceania");
        sut.Settings.ShouldBeNoRepeat(thoughtChildNodeParser);
        sut.AggregationSettings.ShouldBeAggregate(thoughtChildNodeParser);
    }
}
