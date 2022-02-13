namespace IS.Reading.Parsing;

public class StbDocumentReaderTests
{
    private class TestReader : IDocumentLineReader
    {
        private readonly string[] lines;
        private int lineIndex = -1;

        public TestReader(string[] lines)
            => this.lines = lines;

        public int CurrentLineIndex => (lineIndex + 1) * 10;

        public void Dispose()
        {
        }

        public Task<Memory<char>?> ReadLineAsync()
        {
            Memory<char>? result = null;
            if (lineIndex < lines.Length - 1)
            {
                lineIndex++;
                result = lines[lineIndex].ToArray().AsMemory();
            }            
            return Task.FromResult(result);
        }
    }

    private sealed class Tester : IDisposable
    {
        private readonly StbDocumentReader sut;
        public Tester(params string[] lines)
            => sut = new StbDocumentReader(new TestReader(lines));

        public void Dispose() => sut.Dispose();

        public async Task NextShouldBe(int lineNumber, string command, string argument)
        {
            (await sut.ReadAsync()).Should().BeTrue();
            sut.AtEnd.Should().BeFalse();
            sut.CurrentLineIndex.Should().Be(lineNumber);
            sut.Command.Should().Be(command);
            sut.Argument.Should().Be(argument);
        }

        public async Task ShouldBeEnd()
        {
            (await sut.ReadAsync()).Should().BeFalse();
            sut.AtEnd.Should().BeTrue();
        }        
    }

    [Fact]
    public async Task Empty()
    {
        using var tester = new Tester();
        await tester.ShouldBeEnd();
    }

    [Fact]
    public async Task CommandAndArgument()
    {
        using var tester = new Tester("alpha", "beta arg", "gamma argument with spaces");
        await tester.NextShouldBe(10, "alpha", string.Empty);
        await tester.NextShouldBe(20, "beta", "arg");
        await tester.NextShouldBe(30, "gamma", "argument with spaces");
        await tester.ShouldBeEnd();
    }
}
