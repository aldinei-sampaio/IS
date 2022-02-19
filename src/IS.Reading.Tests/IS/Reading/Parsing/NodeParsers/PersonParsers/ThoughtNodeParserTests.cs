namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class ThoughtNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextNodeParser = Helper.FakeParser<IBalloonTextNodeParser>("-");
        var moodNodeParser = Helper.FakeParser<IMoodNodeParser>("#");
        var setNodeParser = Helper.FakeParser<ISetNodeParser>("set");
        var sut = new ThoughtNodeParser(elementParser, balloonTextNodeParser, moodNodeParser, setNodeParser);

        sut.Name.Should().Be("thought");
        sut.IsArgumentRequired.Should().BeFalse();
        sut.BalloonType.Should().Be(BalloonType.Thought);
        sut.Settings.Should().BeOfType<ElementParserSettings.Aggregated>();
        sut.Settings.ChildParsers.Should().BeEquivalentTo(balloonTextNodeParser, moodNodeParser, setNodeParser);
    }
}