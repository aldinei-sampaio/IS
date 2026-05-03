namespace IS.Reading.Variables;

public class VariableDictionary : IVariableDictionary
{
    private readonly Dictionary<string, object> dic = new(StringComparer.OrdinalIgnoreCase);

    public object? this[string name] {
        get
        {
            if (dic.TryGetValue(name, out object? value))
                return value;
            return null;
        }
        set {
            if (value is null)
            {
                if (dic.ContainsKey(name))
                    dic.Remove(name);
            }
            else
            {
                dic[name] = value;
            }
        } 
    }

    public int Count => dic.Count;

    public bool IsSet(string name) => dic.ContainsKey(name);
}
