namespace IS.Reading.Parsing.ConditionParsers;

public class WordReaderFactoryTests
{
    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("a = 5")]
    public void Create(string expression)
    {
        var sut = new WordReaderFactory();
        var result = sut.Create(expression);
        result.Should().BeOfType<WordReader>().Which.Text.Should().Be(expression);
    }
}
