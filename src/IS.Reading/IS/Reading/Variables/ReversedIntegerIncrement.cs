namespace IS.Reading.Variables;

public class ReversedIntegerIncrement : IIntegerIncrement
{
    public ReversedIntegerIncrement(string name, int increment, object? oldValue)
        => (Name, Increment, OldValue) = (name, increment, oldValue);

    public int Increment { get; }

    public string Name { get; }

    public object? OldValue { get; }

    public IVarSet Execute(IVariableDictionary variables)
    {
        variables[Name] = OldValue;
        return new IntegerIncrement(Name, Increment);
    }

    public override string ToString()
        => VarSetHelper.ToString(Name, OldValue);
}
