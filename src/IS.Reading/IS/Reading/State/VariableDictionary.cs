namespace IS.Reading.State;

public class VariableDictionary : IVariableDictionary
{
    private readonly Dictionary<string, int> dic = new(StringComparer.OrdinalIgnoreCase);

    public int Count => dic.Count;

    public int this[string name]
    {
        get {
            if (dic.TryGetValue(name, out var value))
                return value;
            return 0;
        }
        set
        {
            if (value == 0)
            {
                if (dic.ContainsKey(name))
                    dic.Remove(name);
                return;
            }
            dic[name] = value;
        }
    }
}
