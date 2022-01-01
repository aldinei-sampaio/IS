namespace IS.Reading.Variables;

public class VariableDictionary : IVariableDictionary, IIntegerDictionary, IStringDictionary
{
    private readonly IIntegerDictionary integerDictionary;
    private readonly IStringDictionary stringDictionary;

    public VariableDictionary(IIntegerDictionary integerDictionary, IStringDictionary stringDictionary)
    {
        this.integerDictionary = integerDictionary;
        this.stringDictionary = stringDictionary;
    }

    int? IIntegerDictionary.this[string name] { 
        get => integerDictionary[name]; 
        set {
            stringDictionary[name] = null;
            integerDictionary[name] = value;
        } 
    }

    string? IStringDictionary.this[string name] {
        get => stringDictionary[name];
        set
        {
            integerDictionary[name] = null;
            stringDictionary[name] = value;
        }
    }

    public IStringDictionary Strings => this;

    public IIntegerDictionary Integers => this;

    public int Count => Strings.Count + Integers.Count;

    int IIntegerDictionary.Count => stringDictionary.Count;

    bool IIntegerDictionary.IsSet(string name) => integerDictionary.IsSet(name);

    int IStringDictionary.Count => integerDictionary.Count;

    bool IStringDictionary.IsSet(string name) => stringDictionary.IsSet(name);

    public void Unset(string name)
    {
        integerDictionary[name] = null;
        stringDictionary[name] = null;
    }

    public bool IsSet(string name)
        => integerDictionary.IsSet(name) || stringDictionary.IsSet(name);
}
