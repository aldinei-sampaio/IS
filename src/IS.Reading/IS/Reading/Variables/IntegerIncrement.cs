namespace IS.Reading.Variables;

public class IntegerIncrement : IIntegerIncrement
{
    public IntegerIncrement(string name, int increment)
        => (Name, Increment) = (name, increment);

    public int Increment { get; }

    public string Name { get; }

    public IVarSet Execute(IVariableDictionary variables)
    {
        var oldValue = variables[Name];
        var intValue = oldValue as int?;
        variables[Name] = (intValue ?? 0) + Increment;
        return new ReversedIntegerIncrement(Name, Increment, oldValue);
    }
}
