namespace IS.Reading.Parsing.NodeParsers;

public class NarrationNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var childNodeParser = Helper.FakeParser<INarrationChildNodeParser>("-");
        var setNodeParser = Helper.FakeParser<ISetNodeParser>("set");
        var sut = new NarrationNodeParser(elementParser, childNodeParser, setNodeParser);

        sut.Name.Should().Be(">");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.BalloonType.Should().Be(BalloonType.Narration);
        sut.Settings.Should().BeOfType<ElementParserSettings.AggregatedCurrent>();
        sut.Settings.ChildParsers.Should().BeEquivalentTo(childNodeParser, setNodeParser);
    }
}
