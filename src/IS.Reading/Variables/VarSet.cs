namespace IS.Reading.Variables;

public class VarSet(string name, object? value) : IVarSet
{
    public object? Value { get; } = value;

    public string Name { get; } = name;

    public object? Execute(IVariableDictionary variables)
    {
        var oldValue = variables[Name];
        variables[Name] = Value;
        return oldValue;
    }

    public override string ToString()
        => VarSetHelper.ToString(Name, Value);
}
