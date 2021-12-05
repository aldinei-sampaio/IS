using IS.Reading.Parsing.NodeParsers.BalloonParsers;
using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class ThoughtNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextParser = A.Dummy<IBalloonTextParser>();
        var sut = new ThoughtNodeParser(elementParser, balloonTextParser);
        sut.Name.Should().Be("thought");
        var childParser = sut.ChildParser.Should().BeOfType<BalloonTextChildNodeParser>().Which;
        childParser.BalloonType.Should().Be(BalloonType.Thought);
        childParser.Settings.TextParser.Should().BeSameAs(balloonTextParser);
        childParser.Settings.ChildParsers.Count.Should().Be(0);
        childParser.Settings.AttributeParsers.Count.Should().Be(0);
    }
}
