namespace IS.Reading.Variables;

public class StringDictionary : IStringDictionary
{
    private readonly Dictionary<string, string> dic = new(StringComparer.OrdinalIgnoreCase);

    public int Count => dic.Count;

    public string? this[string name]
    {
        get {
            if (dic.TryGetValue(name, out var value))
                return value;
            return null;
        }
        set
        {
            if (value is null)
            {
                if (dic.ContainsKey(name))
                    dic.Remove(name);
                return;
            }
            dic[name] = value;
        }
    }

    public bool IsSet(string name) => dic.ContainsKey(name);
}
