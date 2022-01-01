namespace IS.Reading.Variables;

public class ReversedIntegerIncrement : IIntegerIncrement
{
    public ReversedIntegerIncrement(string name, int increment, int? oldValue)
        => (Name, Increment, OldValue) = (name, increment, oldValue);

    public int Increment { get; }

    public string Name { get; }

    public int? OldValue { get; }

    public IVarSet Execute(IVariableDictionary variables)
    {
        variables.Integers[Name] = OldValue;
        return new IntegerIncrement(Name, Increment);
    }
}
