using System.Text.RegularExpressions;
using System.Xml;

namespace IS.Reading.Parsing.TextParsers;

public class IntegerTextParser : IIntegerTextParser
{
    public string? Parse(XmlReader reader, IParsingContext parsingContext, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = value.Trim();

        if (!Regex.IsMatch(value, @"^\-?\d{1,9}$"))
        { 
            parsingContext.LogError(reader, $"O texto '{value}' não representa um número inteiro válido.");
            return null;
        }

        return value;
    }
}
