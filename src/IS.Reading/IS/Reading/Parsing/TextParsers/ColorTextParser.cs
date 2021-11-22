using System.Text.RegularExpressions;
using System.Xml;

namespace IS.Reading.Parsing.TextParsers;

public class ColorTextParser : IColorTextParser
{
    public string? Parse(XmlReader reader, IParsingContext parsingContext, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        if (!Regex.IsMatch(value, @"^(#[a-f0-9]{6}|black|green|silver|gray|olive|white|yellow|maroon|navy|red|blue|purple|teal|fuchsia|aqua)$", RegexOptions.IgnoreCase))
        {
            parsingContext.LogError(reader, $"O texto '{value}' não representa uma cor válida.");
            return null;
        }

        return value.ToLower();
    }
}
