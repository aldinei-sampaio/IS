using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class TutorialNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var childParser = Helper.FakeParser<ITutorialChildNodeParser>("tutorial");
        var sut = new TutorialNodeParser(elementParser, childParser);
        sut.Name.Should().Be("tutorial");
        sut.ChildParser.Should().Be(childParser);
    }
}
