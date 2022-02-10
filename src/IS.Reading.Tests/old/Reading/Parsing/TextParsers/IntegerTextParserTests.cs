using System.Xml;

namespace IS.Reading.Parsing.ArgumentParsers;

public class IntegerTextParserTests
{
    [Theory]
    [InlineData("0", "0")]
    [InlineData("000000000", "000000000")]
    [InlineData("-0", "-0")]
    [InlineData("1", "1")]
    [InlineData("12345", "12345")]
    [InlineData("999999999", "999999999")]
    [InlineData("-152", "-152")]
    [InlineData("-1", "-1")]
    [InlineData(" 5 ", "5")]
    [InlineData("12\r\n", "12")]
    public void ValidValues(string value, string expected)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();

        var sut = new IntegerArgumentParser();
        var actual = sut.Parse(reader, context, value);
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("0000000000")]
    [InlineData("1000000000")]
    [InlineData("-1000000000")]
    [InlineData("abc")]
    [InlineData("B2F500")]
    [InlineData("#000000")]
    public void InvalidValues(string value)
    {
        var message = $"O texto '{value}' não representa um número inteiro válido.";

        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var sut = new IntegerArgumentParser();
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

        var sut = new IntegerArgumentParser();
        var actual = sut.Parse(reader, context, value);
        actual.Should().Be(string.Empty);
    }
}
