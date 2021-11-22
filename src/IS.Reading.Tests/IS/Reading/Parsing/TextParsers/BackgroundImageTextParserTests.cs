using System.Xml;

namespace IS.Reading.Parsing.TextParsers;

public class BackgroundImageTextParserTests
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
    public void Parse(string value, string expected)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();

        var sut = new BackgroundImageTextParser();
        var actual = sut.Parse(reader, context, value);
        actual.Should().Be(expected);
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

        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>();
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var sut = new BackgroundImageTextParser();
        var actual = sut.Parse(reader, context, value);
        actual.Should().BeNull();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void TextTooLong()
    {
        var value = "12345678901234567890123456789012345678901234567890123456789012345";
        var message = $"O texto contém 65 caracteres, o que excede a quantidade máxima de 64.";

        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>();
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var sut = new BackgroundImageTextParser();
        var actual = sut.Parse(reader, context, value);
        actual.Should().BeNull();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }
}
