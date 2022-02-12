using System.Text;

namespace IS.Reading.Parsing;

public class DocumentLineReaderTests
{
    private class Tester : IDisposable
    {
        public DocumentLineReader Sut { get; }

        public Tester(string text)
        {
            var textReader = new StringReader(text);
            Sut = new DocumentLineReader(textReader);
        }

        public async Task ShouldBeEnd()
        {
            var result = await Sut.ReadLineAsync();
            result.HasValue.Should().BeFalse();
        }

        public async Task NextLineShouldBe(int lineIndex, string text)
        {
            var result = await Sut.ReadLineAsync();
            result.HasValue.Should().BeTrue();
            result.Value.ToString().Should().Be(text);
            Sut.CurrentLineIndex.Should().Be(lineIndex);
        }

        public async Task NextLineShouldBe(int lineIndex, char character, int count)
        {
            var result = await Sut.ReadLineAsync();
            result.HasValue.Should().BeTrue();
            Sut.CurrentLineIndex.Should().Be(lineIndex);
            CompareSpan(result.Value.Span, character, count);
        }

        private static void CompareSpan(Span<char> span, char character, int count)
        {
            span.Length.Should().Be(count);

            var e = span.GetEnumerator();
            var n = 0;
            while (e.MoveNext())
            {
                n++;
                if (e.Current != character)
                    throw new Exception($"Diferença na posição {n}: era esperado '{character}' (código {Convert.ToInt32(character)}) mas foi encontrado '{e.Current}' (código {Convert.ToInt32(e.Current)})");
            }
        }

        public void Dispose()
            => Sut.Dispose();
    }

    [Fact]
    public async Task EmptyText()
    {
        using var tester = new Tester("");
        await tester.ShouldBeEnd();
    }

    [Fact]
    public async Task SingleLine()
    {
        using var tester = new Tester("abra cadabra");
        await tester.NextLineShouldBe(1, "abra cadabra");
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
        using var tester = new Tester(text);
        await tester.NextLineShouldBe(1, "a");
        await tester.NextLineShouldBe(2, "b");
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
        using var tester = new Tester(text);
        await tester.NextLineShouldBe(1, "Linha 1");
        await tester.NextLineShouldBe(2, "Linha 2");
        await tester.NextLineShouldBe(3, "Linha 3");
        await tester.ShouldBeEnd();
    }    

    [Theory]
    [InlineData("\r\n")]
    [InlineData("\r")]
    [InlineData("\n")]
    public async Task BigText(string lineBreak)
    {
        var builder = new StringBuilder();
        for (var n = 0; n < 30; n++)
            builder.Append("1234567890");

        var baseLine = builder.ToString();

        for (var n = 0; n < 9; n++)
        {
            builder.Append(lineBreak);
            builder.Append(baseLine);
        }

        var text = builder.ToString();

        using var tester = new Tester(text);

        await tester.NextLineShouldBe(1, baseLine);
        await tester.NextLineShouldBe(2, baseLine);
        await tester.NextLineShouldBe(3, baseLine);
        await tester.NextLineShouldBe(4, baseLine);
        await tester.NextLineShouldBe(5, baseLine);
        await tester.NextLineShouldBe(6, baseLine);
        await tester.NextLineShouldBe(7, baseLine);
        await tester.NextLineShouldBe(8, baseLine);
        await tester.NextLineShouldBe(9, baseLine);
        await tester.NextLineShouldBe(10, baseLine);

        await tester.ShouldBeEnd();
    }

    [Theory]
    [InlineData(512)]
    [InlineData(600)]
    [InlineData(2048)]
    public async Task RefillBufferCausedByTrim(int indentation)
    {
        var line = "12345678901234567890";

        var builder = new StringBuilder();
        for (var m = 0; m < 2; m++)
        {
            for (var n = 0; n < indentation; n++)
                builder.Append(' ');
            builder.Append(line);
            builder.Append('\n');
        }

        using var tester = new Tester(builder.ToString());

        await tester.NextLineShouldBe(1, line);
        await tester.NextLineShouldBe(2, line);
        await tester.ShouldBeEnd();
    }

    [Fact]
    public async Task RefillBufferCausedByLineBreak()
    {
        var line = "12345678901234567890";

        var builder = new StringBuilder();
        for (var m = 0; m < 2; m++)
        {
            for (var n = 0; n < 250; n++)
                builder.Append("\r\n");
            builder.Append(line);
        }

        using var tester = new Tester(builder.ToString());

        await tester.NextLineShouldBe(251, line);
        await tester.NextLineShouldBe(501, line);
        await tester.ShouldBeEnd();
    }

    [Theory]
    [InlineData(512)]
    [InlineData(600)]
    [InlineData(1200)]
    public async Task FirstLineTooLong(int lineLenght)
    {
        var builder = new StringBuilder();
        for (var n = 0; n < lineLenght; n++)
            builder.Append('a');

        using var tester = new Tester(builder.ToString());

        var ex = await Assert.ThrowsAsync<LineTooLongException>(
            () => tester.Sut.ReadLineAsync()
        );
        ex.LineIndex.Should().Be(1);
    }

    [Theory]
    [InlineData(512)]
    [InlineData(600)]
    [InlineData(1200)]
    public async Task SecondLineTooLong(int lineLenght)
    {
        var builder = new StringBuilder();
        for (var n = 0; n < 500; n++)
            builder.Append('a');
        builder.Append("\r\n");
        for (var n = 0; n < lineLenght; n++)
            builder.Append('b');

        using var tester = new Tester(builder.ToString());

        await tester.NextLineShouldBe(1, 'a', 500);
        var ex = await Assert.ThrowsAsync<LineTooLongException>(
            () => tester.Sut.ReadLineAsync()
        );
        ex.LineIndex.Should().Be(2);
    }

    [Theory]
    [InlineData("Linha1\r\nLinha2\r\nLinha3")]
    [InlineData("Linha1\rLinha2\rLinha3")]
    [InlineData("Linha1\nLinha2\nLinha3")]
    [InlineData("Linha1\r\n    Linha2\r\n    Linha3")]
    [InlineData("Linha1\r    Linha2\r    Linha3")]
    [InlineData("Linha1\n    Linha2\n    Linha3")]
    [InlineData("Linha1\r\n\tLinha2\r\n\t\tLinha3\r\n")]
    [InlineData("Linha1\r\tLinha2\r\t\tLinha3\r")]
    [InlineData("Linha1\n\tLinha2\n\t\tLinha3\n")]
    [InlineData("\t  Linha1 \r\n  \tLinha2\t\r\n   \t\tLinha3 \t\r\n  \t\t")]
    [InlineData("\t  Linha1 \r  \tLinha2\t\r   \t\tLinha3 \t\r  \t\t")]
    [InlineData("\t  Linha1 \n  \tLinha2\t\n   \t\tLinha3 \t\n  \t\t")]
    public async Task IgnoreIndentationAndTrimRight(string text)
    {
        using var tester = new Tester(text);
        await tester.NextLineShouldBe(1, "Linha1");
        await tester.NextLineShouldBe(2, "Linha2");
        await tester.NextLineShouldBe(3, "Linha3");
        await tester.ShouldBeEnd();
    }

    [Theory]
    [InlineData("aa\r\n\r\nbb\r\n\r\n\r\ncc\r\n\r\n")]
    [InlineData("aa\r\rbb\r\r\rcc\r\r")]
    [InlineData("aa\n\nbb\n\n\ncc\n\n")]
    [InlineData("aa\r\n  \r\nbb\r\n  \r\n\r\ncc\r\n  \r\n  ")]
    [InlineData("aa\r  \rbb\r  \r\rcc\r  \r  ")]
    [InlineData("aa\n  \nbb\n  \n\ncc\n  \n  ")]
    [InlineData("aa\r\n\t\r\nbb\r\n\t\r\n\t\r\ncc\r\n\t\r\n\t")]
    [InlineData("aa\r\t\rbb\r\t\r\rcc\r\t\r\t")]
    [InlineData("aa\n\t\nbb\n\t\n\ncc\n\t\n\t")]
    [InlineData("aa\r\n\t \t \r\nbb\r\n\t \t \r\n\r\ncc\r\n\t \t \r\n\t")]
    [InlineData("aa\r\t \t \rbb\r\t \t \r\rcc\r\t \t \r\t \t ")]
    [InlineData("aa\n\t \t \nbb\n\t \t \n\ncc\n\t \t \n\t \t ")]
    public async Task IgnoreEmptyLine(string text)
    {
        using var tester = new Tester(text);
        await tester.NextLineShouldBe(1, "aa");
        await tester.NextLineShouldBe(3, "bb");
        await tester.NextLineShouldBe(6, "cc");
        await tester.ShouldBeEnd();
    }

    [Theory]
    [InlineData("\r\nabc")]
    [InlineData("\rabc")]
    [InlineData("\nabc")]
    public async Task LineBreakOnChar1023(string after1022)
    {
        var builder = new StringBuilder();
        for (var n = 0; n < 1023; n++)
            builder.Append(' ');
        builder.Append(after1022);

        using var tester = new Tester(builder.ToString());
        await tester.NextLineShouldBe(2, "abc");
        await tester.ShouldBeEnd();
    }
}
