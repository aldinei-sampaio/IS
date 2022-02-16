namespace IS.Reading.Parsing.NodeParsers;

public class TutorialNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var balloonTextNodeParser = Helper.FakeParser<IBalloonTextNodeParser>("-");
        var setNodeParser = Helper.FakeParser<ISetNodeParser>("set");
        var sut = new TutorialNodeParser(elementParser, balloonTextNodeParser, setNodeParser);

        sut.Name.Should().Be("tutorial");
        sut.IsArgumentRequired.Should().BeFalse();
        sut.Settings.Should().BeOfType<ElementParserSettings.Aggregated>();
        sut.Settings.ChildParsers.Should().BeEquivalentTo(balloonTextNodeParser, setNodeParser);
    }
}
