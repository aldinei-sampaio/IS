namespace IS.Reading.Parsing.NodeParsers;

public class NarrationNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextNodeParser = Helper.FakeParser<IBalloonTextNodeParser>("-");
        var setNodeParser = Helper.FakeParser<ISetNodeParser>("set");
        var sut = new NarrationNodeParser(elementParser, balloonTextNodeParser, setNodeParser);

        sut.Name.Should().Be("narration");
        sut.IsArgumentRequired.Should().BeFalse();
        sut.BalloonType.Should().Be(BalloonType.Narration);
        sut.Settings.Should().BeOfType<ElementParserSettings.Aggregated>();
        sut.Settings.ChildParsers.Should().BeEquivalentTo(balloonTextNodeParser, setNodeParser);
    }
}
