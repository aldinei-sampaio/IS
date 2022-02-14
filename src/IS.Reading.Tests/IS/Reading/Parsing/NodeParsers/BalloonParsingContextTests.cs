namespace IS.Reading.Parsing.NodeParsers;

public class BalloonParsingContextTests
{
    [Fact]
    public void Initialization()
    {
        var balloonType = BalloonType.Speech;
        var sut = new BalloonParsingContext(balloonType);
        sut.BalloonType.Should().Be(balloonType);
        sut.Nodes.Should().BeEmpty();
    }
}
