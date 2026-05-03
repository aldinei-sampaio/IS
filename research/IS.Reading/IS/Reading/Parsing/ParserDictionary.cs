using System.Collections;
using System.Text.RegularExpressions;

namespace IS.Reading.Parsing;

public class ParserDictionary<T> : IParserDictionary<T> where T : IParser
{
    private readonly List<T> allParsers = new();
    private readonly Dictionary<string, T> dic = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<T> ruleParsers = new();

    public int Count => dic.Count + ruleParsers.Count;

    public T? this[string key]
    {
        get
        {
            foreach (var item in ruleParsers)
            {
                if (Regex.IsMatch(key, item.NameRegex!))
                    return item;
            }

            if (dic.TryGetValue(key, out var value))
                return value;

            return default;
        }
    }

    public void Add(T value)
    {
        allParsers.Add(value);
        if (string.IsNullOrEmpty(value.NameRegex))
            dic.Add(value.Name, value);
        else
            ruleParsers.Add(value);
    }

    public IEnumerator<T> GetEnumerator() => allParsers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
