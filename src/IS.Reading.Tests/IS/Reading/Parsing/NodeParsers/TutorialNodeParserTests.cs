namespace IS.Reading.Parsing.NodeParsers;

public class TutorialNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var childNodeParser = Helper.FakeParser<ITutorialChildNodeParser>("-");
        var setNodeParser = Helper.FakeParser<ISetNodeParser>("set");
        var sut = new TutorialNodeParser(elementParser, childNodeParser, setNodeParser);

        sut.Name.Should().Be("!");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.BalloonType.Should().Be(BalloonType.Tutorial);
        sut.Settings.Should().BeOfType<ElementParserSettings.AggregatedCurrent>();
        sut.Settings.ChildParsers.Should().BeEquivalentTo(childNodeParser, setNodeParser);
    }
}
