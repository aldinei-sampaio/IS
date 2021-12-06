using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class SpeechNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var childParser = Helper.FakeParser<ISpeechChildNodeParser>("speech");
        var sut = new SpeechNodeParser(elementParser, childParser);
        sut.Name.Should().Be("speech");
        sut.ChildParser.Should().BeSameAs(childParser);
    }
}
