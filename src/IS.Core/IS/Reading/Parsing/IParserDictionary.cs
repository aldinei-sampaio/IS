namespace IS.Reading.Parsing;

public interface IParserDictionary<T> where T : IParser
{
    void Add(T value);
    int Count { get; }
    T? this[string key] { get; }
}