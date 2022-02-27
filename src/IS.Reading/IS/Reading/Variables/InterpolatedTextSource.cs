using System.Text;

namespace IS.Reading.Variables;

public struct InterpolatedTextSource : ITextSource
{
    public IEnumerable<IInterpolatedValue> Values { get; }

    public int AproxLength { get; }

    public InterpolatedTextSource(IEnumerable<IInterpolatedValue> values, int aproxLength)
    {
        Values = values;
        AproxLength = aproxLength;
    }

    public override string ToString()
    {
        var builder = new StringBuilder(AproxLength);
        foreach (var item in Values)
            builder.Append(item.ToString());
        return builder.ToString();
    }

    public string Build(IVariableDictionary variables)
    {
        var builder = new StringBuilder(AproxLength);
        foreach (var item in Values)
            builder.Append(item.ToString(variables));
        return builder.ToString();
    }
}
