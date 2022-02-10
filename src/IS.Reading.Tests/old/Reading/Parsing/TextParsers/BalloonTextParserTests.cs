using System.Xml;

namespace IS.Reading.Parsing.ArgumentParsers;

public class BalloonTextParserTests
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData(" ", null)]
    [InlineData("aBc", "aBc")]
    public void Parse(string toParse, string expectedParsed)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();

        var sut = new BalloonTextParser();
        var actual = sut.Parse(reader, context, toParse);
        actual.Should().Be(expectedParsed);
    }
}
