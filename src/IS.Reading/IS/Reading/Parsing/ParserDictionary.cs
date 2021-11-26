namespace IS.Reading.Parsing;

public class ParserDictionary<T> : IParserDictionary<T> where T : IParser
{
    private readonly Dictionary<string, T> dic = new(StringComparer.OrdinalIgnoreCase);

    public int Count => dic.Count;

    public T this[string key] => dic[key];

    public void Add(T value) => dic.Add(value.Name, value);

    public bool TryGet(string key, out T value) => dic.TryGetValue(key, out value!);
}
