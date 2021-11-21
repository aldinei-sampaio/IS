using System.Text.RegularExpressions;
using System.Xml;

namespace IS.Reading.Parsing.Text;

public class ColorTextParser : IColorTextParser
{
    public static ColorTextParser Instance = new();

    private ColorTextParser()
    {
    }

    public string? Parse(XmlReader reader, IParsingContext parsingContext, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!Regex.IsMatch(value, @"^(#[a-f0-9]{6}|black|green|silver|gray|olive|white|yellow|maroon|navy|red|blue|purple|teal|fuchsia|aqua)$"))
        {
            parsingContext.LogError(reader, $"O texto '{value}' não representa uma cor válida.");
            return null;
        }

        return value.Trim();
    }
}
