using System.Text;

namespace IS.Reading.Variables;

internal static class VarSetHelper
{
    public static string ToString(string name, object? value)
    {
        var builder = new StringBuilder();
        builder.Append(name);
        builder.Append('=');
        if (value is null)
        {
            builder.Append("Null");
        }
        else if (value is string sValue)
        {
            builder.Append('\'');
            builder.Append(sValue);
            builder.Append('\'');
        }
        else
        {
            builder.Append(value.ToString());
        }
        return builder.ToString();
    }
}
