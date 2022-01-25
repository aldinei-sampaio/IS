using System.Text;

namespace IS.Reading.Variables;

public class Interpolator : IInterpolator
{
    public IEnumerable<IInterpolatedValue> Values { get; }

    public int AproxLenght { get; }

    public Interpolator(IEnumerable<IInterpolatedValue> values, int aproxLenght)
    {
        Values = values;
        AproxLenght = aproxLenght;
    }

    public string Interpolate(IVariableDictionary variables)
    {
        var builder = new StringBuilder(AproxLenght);
        foreach(var item in Values)
        {
            if (item.IsVariable)
            {
                var value = variables[item.Value];
                if (value is string s)
                    builder.Append(s);
                else if (value is int i)
                    builder.Append(i);                    
            }
            else
            {
                builder.Append(item.Value);
            }
        }
        return builder.ToString();
    }
}
