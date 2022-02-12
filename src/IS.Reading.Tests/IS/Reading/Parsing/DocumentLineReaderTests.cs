using System.Text;

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

    [Fact]
    public async Task BigText()
    {
        var builder = new StringBuilder();
        for (var n = 0; n < 30; n++)
            builder.Append("1234567890");

        var baseLine = builder.ToString();

        for (var n = 0; n < 9; n++)
        {
            builder.AppendLine();
            builder.Append(baseLine);
        }

        var text = builder.ToString();

        var tester = new Tester(text);

        await tester.NextLineShouldBe(baseLine);
        await tester.NextLineShouldBe(baseLine);
        await tester.NextLineShouldBe(baseLine);
        await tester.NextLineShouldBe(baseLine);
        await tester.NextLineShouldBe(baseLine);
        await tester.NextLineShouldBe(baseLine);
        await tester.NextLineShouldBe(baseLine);
        await tester.NextLineShouldBe(baseLine);
        await tester.NextLineShouldBe(baseLine);
        await tester.NextLineShouldBe(baseLine);

        await tester.ShouldBeEnd();
    }

    [Fact]
    public async Task RefillForLineStart()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task IgnoreEmptyLine()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task IgnoreEmptyLine()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task LineTooLong()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task IgnoreIndentation()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task IgnoreEmptyLine()
    {
        throw new NotImplementedException();
    }

}
