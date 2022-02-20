namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class SpeechNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextNodeParser = Helper.FakeParser<ISpeechChildNodeParser>("-");
        var moodNodeParser = Helper.FakeParser<IMoodNodeParser>("#");
        var setNodeParser = Helper.FakeParser<ISetNodeParser>("set");
        var sut = new SpeechNodeParser(elementParser, balloonTextNodeParser, moodNodeParser, setNodeParser);

        sut.Name.Should().Be("-");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.BalloonType.Should().Be(BalloonType.Speech);
        sut.Settings.Should().BeOfType<ElementParserSettings.AggregatedCurrent>();
        sut.Settings.ChildParsers.Should().BeEquivalentTo(balloonTextNodeParser, moodNodeParser, setNodeParser);
    }
}
