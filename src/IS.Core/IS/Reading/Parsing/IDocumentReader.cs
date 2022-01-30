namespace IS.Reading.Parsing;

public interface IDocumentReader : IDisposable
{
    bool AtEnd { get; }
    string ElementName { get; }
    string Argument { get; }
    int CurrentLineIndex { get; }
    Task<bool> ReadAsync();
    IDocumentReader ReadSubtree();
}
