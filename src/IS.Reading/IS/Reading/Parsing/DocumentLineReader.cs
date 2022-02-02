namespace IS.Reading.Parsing;

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
    
    public int CurrentLineIndex { get; private set; }

    public async Task<Memory<char>?> ReadLineAsync()
    {
        for (; ; )
        {
            Memory<char> mem;
            if (bufferIndex > halfLength)
            {
                var charsToMove = bytesRead - bufferIndex - 1;

                for (var n = 0; n < charsToMove; n++)
                    buffer[n] = buffer[n + halfLength];
                bufferIndex = 0;
                var read = await textReader.ReadBlockAsync(buffer, halfLength, halfLength);
                bytesRead = charsToMove + read;
                mem = buffer.AsMemory(0, read > 0 ? read : charsToMove);
            }
            else if (bufferIndex == -1)
            {
                mem = buffer.AsMemory();
                bytesRead = await textReader.ReadBlockAsync(mem);
                if (bytesRead == 0)
                    return null;
            }
            else
            {
                mem = buffer.AsMemory(bufferIndex, bytesRead - bufferIndex - 1);
            }

            var lineStart = GetLineStart(mem.Span);
            if (lineStart >= 0)
            {
                bufferIndex += lineStart;
                var line = ReadLine(mem[lineStart..]);
                if (line.Length > 0)
                {
                    CurrentLineIndex++;
                    return line;
                }
            }
            else
            {
                bufferIndex = -1;
            }
        }
    }


    private Memory<char> ReadLine(Memory<char> mem)
    {
        var span = mem.Span;

        var n = span.IndexOfAny('\r', '\n');

        if (n == -1)
        {
            if (mem.Length >= bufferLength)
                throw new Exception("Linha muito longa.");

            bufferIndex = -1;
            return mem.Trim();
        }

        bufferIndex += n + 1;
        return mem[0..n].Trim();
    }

    private int GetLineStart(ReadOnlySpan<char> span)
    {
        for(var n = 0; n < span.Length; n++)
        {
            switch (span[n])
            {
                case ' ':
                    break;
                case '\r':
                    if (n < span.Length - 1 && span[n + 1] == '\n')
                        n++;
                    CurrentLineIndex++;
                    break;
                case '\n':
                    CurrentLineIndex++;
                    break;
                default:
                    return n;
            }
        }
        return -1;
    }
}