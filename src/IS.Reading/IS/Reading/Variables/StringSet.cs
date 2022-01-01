namespace IS.Reading.Variables;

public class StringSet : IStringSet
{
    public StringSet(string name, string? value)
        => (Name, Value) = (name, value);

    public string? Value { get; }

    public string Name { get; }

    public IVarSet Execute(IVariableDictionary variables)
    {
        var oldValue = variables.Strings[Name];
        variables.Strings[Name] = Value;
        return new StringSet(Name, oldValue);
    }
}
