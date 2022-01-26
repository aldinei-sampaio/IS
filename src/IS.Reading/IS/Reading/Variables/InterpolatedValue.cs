namespace IS.Reading.Variables;

public struct InterpolatedValue : IInterpolatedValue
{
    public string Value { get; } = string.Empty;
    public bool IsVariable { get; }
    public InterpolatedValue(string value, bool isVariable)
    {
        Value = value;
        IsVariable = isVariable;
    }

    public string ToString(IVariableDictionary variables)
    {
        if (!IsVariable)
            return Value;

        var value = variables[Value];
        if (value is string s)
            return s;
        
        if (value is int i)
            return i.ToString();

        return string.Empty;
    }

    public override string ToString() 
        => IsVariable ? $"{{{Value}}}" : Value;
}
