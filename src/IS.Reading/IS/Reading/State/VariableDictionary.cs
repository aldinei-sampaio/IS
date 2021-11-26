namespace IS.Reading.State;

public class VariableDictionary : IVariableDictionary
{
    private readonly Dictionary<string, int> dic = new(StringComparer.OrdinalIgnoreCase);

    public int Get(string name)
    {
        if (dic.TryGetValue(name, out var value))
            return value;
        return 0;
    }
}
