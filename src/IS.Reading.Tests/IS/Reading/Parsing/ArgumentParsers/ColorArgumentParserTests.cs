namespace IS.Reading.Parsing.ArgumentParsers;

public class ColorArgumentParserTests
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
        var sut = new ColorArgumentParser();
        var result = sut.Parse(value);
        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("lightblue")]
    [InlineData("000000")]
    [InlineData("#fff")]
    [InlineData("gibberish")]
    public void InvalidColorName(string value)
    {
        var message = $"O texto '{value}' não representa uma cor válida.";

        var sut = new ColorArgumentParser();
        var result = sut.Parse(value);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(message);
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
        var message = "Era esperado um argumento com a cor.";

        var sut = new ColorArgumentParser();
        var result = sut.Parse(value);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(message);
    }
}
