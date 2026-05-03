using System.Text;

namespace IS.Reading.Variables;

public class IntegerIncrement : IIntegerIncrement
{
    public IntegerIncrement(string name, int increment)
        => (Name, Increment) = (name, increment);

    public int Increment { get; }

    public string Name { get; }

    public object? Execute(IVariableDictionary variables)
    {
        var oldValue = variables[Name];
        var intOldValue = (oldValue as int?) ?? 0;
        var intNewValue = unchecked(intOldValue + Increment);

        if (Increment > 0)
        {
            if (intNewValue < intOldValue)
                intNewValue = int.MaxValue;
        }
        else
        {
            if (intNewValue > intOldValue)
                intNewValue = int.MinValue;
        }

        variables[Name] = intNewValue;
        return oldValue;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append(Name);
        switch (Increment)
        {
            case 1:
                builder.Append("++");
                break;
            case -1:
                builder.Append("--");
                break;
            case < 0:
                builder.Append("-=");
                builder.Append(Increment.ToString("#;#"));
                break;
            default:
                builder.Append("+=");
                builder.Append(Increment.ToString());
                break;
        }
        return builder.ToString();
    }
}
