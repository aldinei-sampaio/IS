
namespace IS.Reading.Parsing;

public interface IDocumentLineReader : IDisposable
{
    int CurrentLineIndex { get; }

    Task<Memory<char>?> ReadLineAsync();
}
