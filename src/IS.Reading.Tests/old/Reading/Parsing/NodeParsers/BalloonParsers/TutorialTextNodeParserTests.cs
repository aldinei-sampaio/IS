using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class TutorialTextNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextParser = A.Dummy<IBalloonTextParser>();

        var sut = new TutorialTextNodeParser(elementParser, balloonTextParser);
        sut.Name.Should().Be("tutorial");
        sut.BalloonType.Should().Be(BalloonType.Tutorial);
        sut.Settings.ShouldBeNormal(balloonTextParser);
        sut.Should().BeAssignableTo<GenericTextNodeParserBase>();
    }
}
