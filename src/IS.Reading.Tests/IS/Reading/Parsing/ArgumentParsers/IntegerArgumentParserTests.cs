namespace IS.Reading.Parsing.ArgumentParsers;

public class IntegerArgumentParserTests
{
    [Theory]
    [InlineData("0", 0)]
    [InlineData("000000000", 0)]
    [InlineData("-0", 0)]
    [InlineData("1", 1)]
    [InlineData("12345", 12345)]
    [InlineData("999999999", 999999999)]
    [InlineData("-152", -152)]
    [InlineData("-1", -1)]
    [InlineData(" 5 ", 5)]
    [InlineData("12\r\n", 12)]
    [InlineData("2147483647", 2147483647)]
    [InlineData("-2147483648", -2147483648)]
    public void ValidValues(string value, int expected)
    {
        var sut = new IntegerArgumentParser();
        var result = sut.Parse(value, int.MinValue, int.MaxValue);
        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("2147483648")]
    [InlineData("-2147483649")]
    [InlineData("abc")]
    [InlineData("B2F500")]
    [InlineData("#000000")]
    public void InvalidValues(string value)
    {
        var message = $"O texto '{value}' não representa um número inteiro válido.";

        var sut = new IntegerArgumentParser();
        var result = sut.Parse(value, int.MinValue, int.MaxValue);
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
        var message = $"Era esperado um argumento com um número inteiro entre {int.MinValue} e {int.MaxValue}.";

        var sut = new IntegerArgumentParser();
        var result = sut.Parse(value, int.MinValue, int.MaxValue);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(message);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-500")]
    [InlineData("5")]
    [InlineData("9382")]
    public void OutsideAllowedRange(string value)
    {
        var message = "O valor precisa estar entre 2 e 4.";

        var sut = new IntegerArgumentParser();
        var result = sut.Parse(value, 2, 4);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(message);
    }

    [Theory]
    [InlineData("2", 2)]
    [InlineData("3", 3)]
    [InlineData("4", 4)]
    public void IssideAllowedRange(string value, int expected)
    {
        var sut = new IntegerArgumentParser();
        var result = sut.Parse(value, 2, 4);
        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(expected);
    }
}
