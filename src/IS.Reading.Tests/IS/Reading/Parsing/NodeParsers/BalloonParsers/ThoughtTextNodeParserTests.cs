using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class ThoughtTextNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextParser = A.Dummy<IBalloonTextParser>();

        var sut = new ThoughtTextNodeParser(elementParser, balloonTextParser);
        sut.Name.Should().Be("thought");
        sut.BalloonType.Should().Be(BalloonType.Thought);
        sut.Settings.ShouldBeNormal(balloonTextParser);
    }
}
