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
}
