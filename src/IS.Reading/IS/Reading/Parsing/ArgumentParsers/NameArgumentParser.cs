using System.Text;

namespace IS.Reading.Parsing.ArgumentParsers;

public class NameArgumentParser : INameArgumentParser
{
    public Result<string> Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail<string>("Era esperado um argumento.");

        var result = Format(value);

        if (result.Length > 64)
            return Result.Fail<string>($"O texto contém {result.Length} caracteres, o que excede a quantidade máxima de 64.");

        return Result.Ok(result);
    }

    private static string Format(string value)
    {
        var trimmed = value.AsSpan().Trim();

        var builder = new StringBuilder(trimmed.Length);
        foreach(var c in trimmed)
        {
            if (c != '\r' && c != '\n')
                builder.Append(char.ToLower(c));
        }

        return builder.ToString();
    }
}