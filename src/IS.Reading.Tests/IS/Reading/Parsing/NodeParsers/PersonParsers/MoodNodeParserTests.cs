using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class MoodNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IMoodTextParser moodTextParser;
    private readonly MoodNodeParser sut;

    public MoodNodeParserTests()
    {
        elementParser = A.Dummy<IElementParser>();
        moodTextParser = A.Dummy<IMoodTextParser>();

        sut = new MoodNodeParser(elementParser, moodTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("mood");
        sut.Settings.ShouldBeNormal(moodTextParser);
    }

    [Fact]
    public async Task ParseAsyncShouldReturnMoodNode()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = "Happy");

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<MoodNode>(i => i.MoodType.Should().Be(MoodType.Happy));
    }

    [Fact]
    public async Task ParseAsyncShouldReturnNullWhenParserReturnsNullText()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);
        parentContext.ShouldBeEmpty();
    }
}
