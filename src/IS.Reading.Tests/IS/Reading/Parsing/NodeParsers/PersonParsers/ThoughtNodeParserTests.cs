using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class ThoughtNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var childParser = Helper.FakeParser<IThoughtChildNodeParser>("thought");
        var sut = new ThoughtNodeParser(elementParser, childParser);
        sut.Name.Should().Be("thought");
        sut.ChildParser.Should().BeSameAs(childParser);
    }
}
