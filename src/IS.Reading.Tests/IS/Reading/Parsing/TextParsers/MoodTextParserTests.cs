using System.Xml;

namespace IS.Reading.Parsing.TextParsers;

public class MoodTextParserTests
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData(" ", null)]
    [InlineData("abc", null)]
    [InlineData("happy", "Happy")]
    [InlineData("SAD", "Sad")]
    [InlineData("  normal  ", "Normal")]
    [InlineData("Surprised\r\n", "Surprised")]
    [InlineData("Angry", "Angry")]
    public void Parse(string toParse, string expectedParsed)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();

        var parser = new MoodTextParser();
        var actual = parser.Parse(reader, context, toParse);
        actual.Should().Be(expectedParsed);
    }
}
