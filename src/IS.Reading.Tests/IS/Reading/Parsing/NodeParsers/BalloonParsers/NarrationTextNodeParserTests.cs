using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class NarrationTextNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextParser = A.Dummy<IBalloonTextParser>();

        var sut = new NarrationTextNodeParser(elementParser, balloonTextParser);
        sut.Name.Should().Be("narration");
        sut.BalloonType.Should().Be(BalloonType.Narration);
        sut.Settings.ShouldBeNormal(balloonTextParser);
    }
}
