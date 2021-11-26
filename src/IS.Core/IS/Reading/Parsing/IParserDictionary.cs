namespace IS.Reading.Parsing;

public interface IParserDictionary<T> where T : IParser
{
    void Add(T value);
    bool TryGet(string name, out T value);
    int Count { get; }
    T this[string key] { get; }
}