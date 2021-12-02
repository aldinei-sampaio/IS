using IS.Reading.Parsing.NodeParsers.Helpers;
using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class TutorialNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextParser = A.Dummy<IBalloonTextParser>();
        var sut = new TutorialNodeParser(elementParser, balloonTextParser);
        sut.Name.Should().Be("tutorial");
        var childParser = sut.ChildParser.Should().BeOfType<BalloonTextChildNodeParser>().Which;
        childParser.BalloonType.Should().Be(BalloonType.Tutorial);
        childParser.Settings.TextParser.Should().BeSameAs(balloonTextParser);
        childParser.Settings.ChildParsers.Count.Should().Be(0);
        childParser.Settings.AttributeParsers.Count.Should().Be(0);
    }
}
