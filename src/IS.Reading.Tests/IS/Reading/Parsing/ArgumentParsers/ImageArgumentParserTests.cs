namespace IS.Reading.Parsing.ArgumentParsers;

public class ImageArgumentParserTests
{
    [Theory]
    [InlineData("abc", "abc")]
    [InlineData("AB_C", "AB_C")]
    [InlineData(" ab.c  ", "ab.c")]
    [InlineData("a\rb", "ab")]
    [InlineData("a\r\nb", "ab")]
    [InlineData("a\nb", "ab")]
    [InlineData("\r\r", null)]
    [InlineData("\r\n\r\n", null)]
    [InlineData("\n\n\n", null)]
    [InlineData("", null)]
    [InlineData(null, null)]
    [InlineData("    ", null)]
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234", "1234567890123456789012345678901234567890123456789012345678901234")]
    public void Parse(string value, string expected)
    {
        var sut = new ImageArgumentParser();
        var result = sut.Parse(value);

        if (expected is null)
        {
            result.IsOk.Should().BeFalse();
        }
        else
        {
            result.IsOk.Should().BeTrue();
            result.Value.Should().Be(expected);
        }
    }
    
    [Theory]
    [InlineData("@t")]
    [InlineData("a-b")]
    [InlineData("a=b")]
    [InlineData("(a")]
    [InlineData("a)")]
    [InlineData("a&b")]
    [InlineData("a!b")]
    [InlineData(@"a\b")]
    [InlineData("a+b")]
    [InlineData("a*b")]
    [InlineData("a/b")]
    [InlineData("a#b")]
    [InlineData("a|b")]
    [InlineData("a{b")]
    [InlineData("a}b")]
    [InlineData("a[b")]
    [InlineData("a]b")]
    [InlineData("a:b")]
    [InlineData("a;b")]
    [InlineData("a<b")]
    [InlineData("a>b")]
    [InlineData("a\"b")]
    [InlineData("a'b")]
    public void InvalidCharacters(string value)
    {
        var message = $"O texto '{value}' contém caracteres inválidos.";

        var sut = new ImageArgumentParser();
        var result = sut.Parse(value);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(message);
    }

    [Fact]
    public void TextTooLong()
    {
        var value = "12345678901234567890123456789012345678901234567890123456789012345";
        var message = $"O texto contém 65 caracteres, o que excede a quantidade máxima de 64.";

        var sut = new ImageArgumentParser();
        var result = sut.Parse(value);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Empty(string value)
    {
        var message = "Era esperado um argumento com o nome da imagem.";

        var sut = new ImageArgumentParser();
        var result = sut.Parse(value);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(message);
    }
}
