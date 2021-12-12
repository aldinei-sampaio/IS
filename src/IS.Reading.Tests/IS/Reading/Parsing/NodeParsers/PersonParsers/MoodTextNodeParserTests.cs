using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class MoodTextNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IMoodTextParser moodTextParser;
    private readonly MoodTextNodeParser sut;

    public MoodTextNodeParserTests()
    {
        elementParser = A.Dummy<IElementParser>();
        moodTextParser = A.Dummy<IMoodTextParser>();
        sut = new(elementParser, moodTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("mood");
        sut.Settings.ShouldBeNormal(moodTextParser);
        sut.Should().BeAssignableTo<GenericTextNodeParserBase>();
    }
}
