using System.Text.RegularExpressions;

namespace IS.Reading.Parsing.ArgumentParsers;

public class ColorArgumentParser : IColorArgumentParser
{
    public Result<string> Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail<string>("Era esperado um argumento com a cor.");

        if (!Regex.IsMatch(value, @"^(#[a-f0-9]{6}|black|green|silver|gray|olive|white|yellow|maroon|navy|red|blue|purple|teal|fuchsia|aqua)$", RegexOptions.IgnoreCase))
            return Result.Fail<string>($"O texto '{value}' não representa uma cor válida.");

        return Result.Ok(value.ToLower());
    }
}
