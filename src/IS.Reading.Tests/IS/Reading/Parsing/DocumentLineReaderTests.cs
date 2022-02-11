namespace IS.Reading.Parsing;

public class DocumentLineReaderTests
{
    private class Tester
    {
        private readonly DocumentLineReader sut;
        public Tester(string text)
        {
            var textReader = new StringReader(text);
            sut = new DocumentLineReader(textReader);
        }

        public async Task ShouldBeEnd()
        {
            var result = await sut.ReadLineAsync();
            result.HasValue.Should().BeFalse();
        }

        public async Task NextLineShouldBe(string text)
        {
            var result = await sut.ReadLineAsync();
            result.HasValue.Should().BeTrue();
            result.Value.ToString().Should().Be(text);
        }
    }

    [Fact]
    public async Task EmptyText()
    {
        var tester = new Tester("");
        await tester.ShouldBeEnd();
    }

    [Fact]
    public async Task SingleLine()
    {
        var tester = new Tester("abra cadabra");
        await tester.NextLineShouldBe("abra cadabra");
        await tester.ShouldBeEnd();
    }

    [Theory]
    [InlineData("a\r\nb")]
    [InlineData("a\rb")]
    [InlineData("a\nb")]
    [InlineData("a\r\nb\r\n")]
    [InlineData("a\rb\r")]
    [InlineData("a\nb\n")]
    public async Task TwoLines(string text)
    {
        var tester = new Tester(text);
        await tester.NextLineShouldBe("a");
        await tester.NextLineShouldBe("b");
        await tester.ShouldBeEnd();
    }

    [Theory]
    [InlineData("Linha 1\r\nLinha 2\r\nLinha 3")]
    [InlineData("Linha 1\rLinha 2\rLinha 3")]
    [InlineData("Linha 1\nLinha 2\nLinha 3")]
    [InlineData("Linha 1\r\nLinha 2\r\nLinha 3\r\n")]
    [InlineData("Linha 1\rLinha 2\rLinha 3\r")]
    [InlineData("Linha 1\nLinha 2\nLinha 3\n")]
    public async Task ThreeLines(string text)
    {
        var tester = new Tester(text);
        await tester.NextLineShouldBe("Linha 1");
        await tester.NextLineShouldBe("Linha 2");
        await tester.NextLineShouldBe("Linha 3");
        await tester.ShouldBeEnd();
    }
}
