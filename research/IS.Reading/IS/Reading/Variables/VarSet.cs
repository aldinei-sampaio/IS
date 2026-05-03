namespace IS.Reading.Variables;

public class VarSet : IVarSet
{
    public VarSet(string name, object? value)
        => (Name, Value) = (name, value);

    public object? Value { get; }

    public string Name { get; }

    public object? Execute(IVariableDictionary variables)
    {
        var oldValue = variables[Name];
        variables[Name] = Value;
        return oldValue;
    }

    public override string ToString()
        => VarSetHelper.ToString(Name, Value);
}
