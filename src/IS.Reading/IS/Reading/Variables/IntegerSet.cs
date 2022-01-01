namespace IS.Reading.Variables;

public class IntegerSet : IIntegerSet
{
    public IntegerSet(string name, int? value)
        => (Name, Value) = (name, value);

    public int? Value { get; }

    public string Name { get; }

    public IVarSet Execute(IVariableDictionary variables)
    {
        var oldValue = variables.Integers[Name];
        variables.Integers[Name] = Value;
        return new IntegerSet(Name, oldValue);
    }
}
