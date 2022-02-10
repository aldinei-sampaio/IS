using System.Xml;

namespace IS.Reading.Parsing.ArgumentParsers;

public class ColorTextParserTests
{
    [Theory]
    [InlineData("black", "black")]
    [InlineData("GREEN", "green")]
    [InlineData("Silver", "silver")]
    [InlineData("gray", "gray")]
    [InlineData("olive", "olive")]
    [InlineData("white", "white")]
    [InlineData("yellow", "yellow")]
    [InlineData("maroon", "maroon")]
    [InlineData("navy", "navy")]
    [InlineData("red", "red")]
    [InlineData("blue", "blue")]
    [InlineData("PURPLE", "purple")]
    [InlineData("teal", "teal")]
    [InlineData("fuchsia", "fuchsia")]
    [InlineData("aqua", "aqua")]
    [InlineData("#000000", "#000000")]
    [InlineData("#AAAAAA", "#aaaaaa")]
    [InlineData("#f2f2F2", "#f2f2f2")]
    public void Parse(string value, string expected)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();

        var sut = new ColorTextParser();
        var actual = sut.Parse(reader, context, value);
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("lightblue")]
    [InlineData("000000")]
    [InlineData("#fff")]
    [InlineData("gibberish")]
    public void InvalidColorName(string value)
    {
        var message = $"O texto '{value}' não representa uma cor válida.";

        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var sut = new ColorTextParser();
        var actual = sut.Parse(reader, context, value);
        actual.Should().BeNull();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\r\n")]
    [InlineData("\t")]
    public void Empty(string value)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());

        var sut = new ColorTextParser();
        var actual = sut.Parse(reader, context, value);
        actual.Should().Be(string.Empty);
    }
}
