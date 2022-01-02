namespace IS.Reading.Variables;

public class VarUnset : IVarUnset
{
    public VarUnset(string name)
        => Name = name;

    public string Name { get; }

    public IVarSet Execute(IVariableDictionary variables)
    {
        var oldStringValue = variables.Strings[Name];
        var oldIntegerValue = variables.Integers[Name];

        variables.Unset(Name);

        return new ReversedVarUnset(Name, oldStringValue, oldIntegerValue);
    }
}
