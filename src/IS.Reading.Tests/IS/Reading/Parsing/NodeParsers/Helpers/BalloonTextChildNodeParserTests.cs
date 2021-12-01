using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.Helpers;

public class BalloonTextChildNodeParserTests
{
    private readonly IElementParser elementParser = A.Fake<IElementParser>(i => i.Strict());
    private readonly IBalloonTextParser balloonTextParser = A.Dummy<IBalloonTextParser>();

    [Fact]
    public void Initialization()
    {
        var sut = new BalloonTextChildNodeParser(
            elementParser, 
            balloonTextParser, 
            BalloonType.Narration, 
            "narration"
        );

        sut.Name.Should().Be("narration");
        sut.BalloonType.Should().Be(BalloonType.Narration);        
        sut.Settings.TextParser.Should().BeSameAs(balloonTextParser);
        sut.Settings.AttributeParsers.Count.Should().Be(0);
        sut.Settings.ChildParsers.Count.Should().Be(0);
    }

    [Fact]
    public async Task ShouldReturnNodeWhenParsedTextIsNotNull()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();

        var sut = new BalloonTextChildNodeParser(
            elementParser,
            balloonTextParser,
            BalloonType.Tutorial,
            "tutorial"
        );

        var parsedData = A.Dummy<IElementParsedData>();
        parsedData.Text = "abc";
        
        A.CallTo(() => elementParser.ParseAsync(A<XmlReader>.Ignored, context, sut.Settings)).Returns(parsedData);

        var ret = await sut.ParseAsync(reader, context);
        var parsed = ret.Should().BeOfType<BalloonTextNode>().Which;
        parsed.Text.Should().Be("abc");
        parsed.BalloonType.Should().Be(BalloonType.Tutorial);
    }

    [Fact]
    public async Task ShouldReturnNullWhenParsedTextIsNull()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();

        var sut = new BalloonTextChildNodeParser(
            elementParser,
            balloonTextParser,
            BalloonType.Speech,
            "speech"
        );

        var parsedData = A.Dummy<IElementParsedData>();
        parsedData.Text = null;

        A.CallTo(() => elementParser.ParseAsync(A<XmlReader>.Ignored, context, sut.Settings)).Returns(parsedData);

        var ret = await sut.ParseAsync(reader, context);
        ret.Should().BeNull();
    }
}
