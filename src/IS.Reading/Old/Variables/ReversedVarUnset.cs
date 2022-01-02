namespace IS.Reading.Variables;

public class ReversedVarUnset : IVarUnset
{
    public ReversedVarUnset(string name, string? oldStringValue, int? oldIntegerValue)
    {
        Name = name;
        OldStringValue = oldStringValue;
        OldIntegerValue = oldIntegerValue;
    }

    public string Name { get; }
    public string? OldStringValue { get; }
    public int? OldIntegerValue { get; }

    public IVarSet Execute(IVariableDictionary variables)
    {
        if (OldStringValue is not null)
            variables.Strings[Name] = OldStringValue;
        else
            variables.Integers[Name] = OldIntegerValue;
        return new VarUnset(Name);
    }
}