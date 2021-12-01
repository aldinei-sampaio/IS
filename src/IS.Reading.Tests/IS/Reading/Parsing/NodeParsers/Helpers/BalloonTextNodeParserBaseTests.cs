using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers.Helpers;

public class BalloonTextNodeParserBaseTests
{
    private class TestClass : BalloonTextNodeParserBase
    {
        public TestClass(string name, BalloonType balloonType, IElementParser elementParser, IBalloonTextParser balloonTextParser) 
            : base(name, balloonType, elementParser, balloonTextParser)
        {
        }
    }

    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextparser = A.Dummy<IBalloonTextParser>();
        var sut = new TestClass("chorume", BalloonType.Thought, elementParser, balloonTextparser);
        sut.Name.Should().Be("chorume");
        sut.ChildParser.Name.Should().Be("chorume");
        sut.ChildParser.BalloonType.Should().Be(BalloonType.Thought);
        sut.ChildParser.Settings.TextParser.Should().BeSameAs(balloonTextparser);
        sut.ChildParser.Settings.AttributeParsers.Count.Should().Be(0);
        sut.ChildParser.Settings.ChildParsers.Count.Should().Be(0);
    }
}
