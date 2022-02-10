using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class SpeechTextNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextParser = A.Dummy<IBalloonTextParser>();

        var sut = new SpeechTextNodeParser(elementParser, balloonTextParser);
        sut.Name.Should().Be("speech");
        sut.BalloonType.Should().Be(BalloonType.Speech);
        sut.Settings.ShouldBeNormal(balloonTextParser);
        sut.Should().BeAssignableTo<GenericTextNodeParserBase>();
    }
}
