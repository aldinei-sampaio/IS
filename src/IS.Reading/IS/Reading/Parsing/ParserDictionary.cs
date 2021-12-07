using System.Text.RegularExpressions;

namespace IS.Reading.Parsing;

public class ParserDictionary<T> : IParserDictionary<T> where T : IParser
{
    private readonly Dictionary<string, T> dic = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<T> ruleParsers = new();

    public int Count => dic.Count + ruleParsers.Count;

    public T this[string key]
    {
        get
        {
            if (TryGet(key, out var value))
                return value;
            throw new KeyNotFoundException(key);
        }
    }

    public void Add(T value)
    {
        if (string.IsNullOrEmpty(value.NameRegex))
            dic.Add(value.Name, value);
        else
            ruleParsers.Add(value);
    }

    public bool TryGet(string key, out T value)
    {
        foreach(var item in ruleParsers)
        {
            if (Regex.IsMatch(item.NameRegex!, key))
            {
                value = item;
                return true;
            }
        }

        return dic.TryGetValue(key, out value!);
    }
}
