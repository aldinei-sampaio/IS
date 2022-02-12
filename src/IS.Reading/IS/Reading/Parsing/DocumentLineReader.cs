namespace IS.Reading.Parsing;

public class LineTooLongException : Exception
{
    public LineTooLongException(int lineIndex) : base($"Linha {lineIndex}: Linha muito longa.")
        => LineIndex = lineIndex;

    public int LineIndex { get; }
}

public class DocumentLineReader : IDisposable
{
    private readonly TextReader textReader;

    private const int bufferLength = 1024;
    private const int halfLength = bufferLength / 2;

    private char[] buffer = new char[bufferLength];
    private int bytesRead = 0;
    private int bufferIndex = 0;

    public DocumentLineReader(TextReader textReader)
        => this.textReader = textReader;

    public void Dispose()
        => textReader.Dispose();

    public int CurrentLineIndex { get; private set; } = 1;

    private async Task<Memory<char>> RefillBufferAsync()
    {
        var charsToMove = bytesRead - bufferIndex;
        var charsToRead = bufferLength - charsToMove;

        for (var n = 0; n < charsToMove; n++)
            buffer[n] = buffer[bufferIndex + n];

        bufferIndex = 0;
        var read = await textReader.ReadBlockAsync(buffer, charsToMove, charsToRead);
        bytesRead = charsToMove + read;

        return buffer.AsMemory(0, bytesRead);
    }

    public async Task<Memory<char>?> ReadLineAsync()
    {
        for (; ; )
        {
            Memory<char> mem;
            if (bufferIndex > halfLength)
            {
                mem = await RefillBufferAsync();
            }
            else if (bufferIndex == bytesRead)
            {
                if (bytesRead > 0 && bytesRead < bufferLength)
                    return null;

                bytesRead = await textReader.ReadBlockAsync(buffer, 0, bufferLength);
                if (bytesRead == 0)
                    return null;

                mem = buffer.AsMemory(0, bytesRead);
            }
            else
            {
                mem = buffer.AsMemory(bufferIndex, bytesRead - bufferIndex);
            }

            var isFinalBuffer = bytesRead < bufferLength;
            var lineStart = GetLineStart(mem.Span, isFinalBuffer);
            if (lineStart >= 0)
            {
                bufferIndex += lineStart;
                mem = mem[lineStart..];
                if (mem.Length <= halfLength)
                    mem = await RefillBufferAsync();
                return ReadLine(mem);
            }
            else if (lineStart == -2)
            {
                bufferIndex += mem.Length - 1;
            }
            else
            {
                bufferIndex = bytesRead;
            }
        }
    }

    private Memory<char> ReadLine(Memory<char> mem)
    {
        var span = mem.Span;
        var n = span.IndexOfAny('\r', '\n');

        if (n == -1)
        {
            if (mem.Length >= halfLength)
                throw new LineTooLongException(CurrentLineIndex);

            bufferIndex = bytesRead;
            return mem.TrimEnd();
        }

        bufferIndex += n;
        return mem[0..n].TrimEnd();
    }

    private int GetLineStart(ReadOnlySpan<char> span, bool isFinalBuffer)
    {
        for(var n = 0; n < span.Length; n++)
        {
            var c = span[n];
            if (char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.SpaceSeparator)
            {
                switch (c)
                {
                    case '\t':
                        break;
                    case '\r':
                        if (n == span.Length - 1)
                        {
                            if (!isFinalBuffer)
                                return -2;
                        }
                        else if (span[n + 1] == '\n')
                        {
                            n++;
                        }
                        CurrentLineIndex++;
                        break;
                    case '\n':
                        CurrentLineIndex++;
                        break;
                    default:
                        return n;
                }
            }
        }
        return -1;
    }
}