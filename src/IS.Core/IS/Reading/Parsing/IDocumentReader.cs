namespace IS.Reading.Parsing;

public interface IDocumentReader : IDisposable
{
    bool AtEnd { get; }
    string Command { get; }
    string Argument { get; }
    int CurrentLineIndex { get; }
    Task<bool> ReadAsync();
}
