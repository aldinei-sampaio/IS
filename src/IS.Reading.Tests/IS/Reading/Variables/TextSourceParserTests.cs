namespace IS.Reading.Variables;

public class TextSourceParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData("Hello, World!")]
    [InlineData("Prezado sr. {Nome}")]
    [InlineData("{Area} {DDD} {Fone}")]
    public void Success(string text)
    {
        var sut = new TextSourceParser();
        var result = sut.Parse(text);
        result.IsError.Should().BeFalse();
        result.TextSource.ToString().Should().Be(text);
    }
}
