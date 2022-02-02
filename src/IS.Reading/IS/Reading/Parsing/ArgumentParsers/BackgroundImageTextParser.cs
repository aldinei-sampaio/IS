using System.Text.RegularExpressions;

namespace IS.Reading.Parsing.ArgumentParsers;

public class BackgroundImageTextParser : IBackgroundImageTextParser
{
    public string? Parse(IDocumentReader reader, IParsingContext parsingContext, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (Regex.IsMatch(value, @"[^A-Za-z0-9_\r\n\. ]"))
        {
            parsingContext.LogError(reader, $"O texto '{value}' contém caracteres inválidos.");
            return null;
        }

        var result = value.ReplaceLineEndings(string.Empty).Trim();

        if (result.Length <= 64)
            return result;

        parsingContext.LogError(reader, $"O texto contém {result.Length} caracteres, o que excede a quantidade máxima de 64.");
        return null;
    }
}
