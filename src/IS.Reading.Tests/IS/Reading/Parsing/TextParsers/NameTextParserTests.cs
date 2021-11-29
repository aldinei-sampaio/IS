using System.Xml;

namespace IS.Reading.Parsing.TextParsers;

public class NameTextParserTests
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData(" ", null)]
    [InlineData("alpha", "alpha")]
    [InlineData("ALPHA", "alpha")]
    [InlineData("\r\nalpha\r\n", "alpha")]
    [InlineData(" alpha ", "alpha")]
    [InlineData("al\rp\nha", "alpha")]
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234", "1234567890123456789012345678901234567890123456789012345678901234")]
    public void Parse(string toParse, string expectedParsed)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();

        var parser = new NameTextParser();
        var actual = parser.Parse(reader, context, toParse);
        actual.Should().Be(expectedParsed);
    }

    [Theory]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345")]
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234567890")]
    public void MaxLength(string toParse)
    {
        var errorMessage = $"O texto contém {toParse.Length} caracteres, o que excede a quantidade máxima de 64.";

        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();

        var parser = new NameTextParser();
        var ret = parser.Parse(reader, context, toParse);
        ret.Should().BeNull();

        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
    }

}
