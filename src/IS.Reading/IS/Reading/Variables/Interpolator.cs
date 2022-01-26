using System.Text;

namespace IS.Reading.Variables;

public class Interpolator : IInterpolator
{
    public IEnumerable<IInterpolatedValue> Values { get; }

    public int AproxLength { get; }

    public Interpolator(IEnumerable<IInterpolatedValue> values, int aproxLength)
    {
        Values = values;
        AproxLength = aproxLength;
    }

    public string ToString(IVariableDictionary variables)
    {
        var builder = new StringBuilder(AproxLength);
        foreach(var item in Values)
            builder.Append(item.ToString(variables));
        return builder.ToString();
    }

    public override string ToString()
    {
        var builder = new StringBuilder(AproxLength);
        foreach (var item in Values)
            builder.Append(item.ToString());
        return builder.ToString();
    }
}
