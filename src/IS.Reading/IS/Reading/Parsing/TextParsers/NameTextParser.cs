using System.Text;
using System.Xml;

namespace IS.Reading.Parsing.TextParsers;

public class NameTextParser : INameTextParser
{
    public string? Parse(XmlReader reader, IParsingContext parsingContext, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var result = Format(value);

        if (result.Length <= 64)
            return result;

        parsingContext.LogError(reader, $"O texto contém {result.Length} caracteres, o que excede a quantidade máxima de 64.");
        return null;
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