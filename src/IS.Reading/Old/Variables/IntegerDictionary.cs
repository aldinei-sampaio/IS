namespace IS.Reading.Variables;

public class IntegerDictionary : IIntegerDictionary
{
    private readonly Dictionary<string, int> dic = new(StringComparer.OrdinalIgnoreCase);

    public int Count => dic.Count;

    public int? this[string name]
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
            dic[name] = value.Value;
        }
    }

    public bool IsSet(string name) => dic.ContainsKey(name);
}
