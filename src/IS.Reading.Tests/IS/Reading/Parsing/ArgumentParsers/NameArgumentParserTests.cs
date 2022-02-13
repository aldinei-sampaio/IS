namespace IS.Reading.Parsing.ArgumentParsers;

public class NameArgumentParserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void EmptyValues(string toParse)
    {
        var errorMessage = "Era esperado um argumento.";

        var parser = new NameArgumentParser();
        var result = parser.Parse(toParse);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Theory]
    [InlineData("alpha", "alpha")]
    [InlineData("ALPHA", "alpha")]
    [InlineData("\r\nalpha\r\n", "alpha")]
    [InlineData(" alpha ", "alpha")]
    [InlineData("al\rp\nha", "alpha")]
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234", "1234567890123456789012345678901234567890123456789012345678901234")]
    public void ValidValues(string toParse, string expectedParsed)
    {
        var parser = new NameArgumentParser();
        var result = parser.Parse(toParse);
        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(expectedParsed);
    }

    [Theory]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345")]
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234567890")]
    public void TextTooLong(string toParse)
    {
        var errorMessage = $"O texto contém {toParse.Length} caracteres, o que excede a quantidade máxima de 64.";

        var parser = new NameArgumentParser();
        var result = parser.Parse(toParse);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }

}
