using System.Text.RegularExpressions;
using System.Xml;

namespace IS.Reading.Parsing.TextParsers;

public class VarSetTextParser : IVarSetTextParser
{
    /// <summary>
    /// name=1
    /// name+=1
    /// name-=1
    /// </summary>
    private const string IntegerSetPattern = /* lang=regex */ @"^[A-Za-z_]+\s?(=|\+=|-=)\s?\d{1,9}$";
    /// <summary>
    /// name++
    /// name--
    /// </summary>
    private const string IntegerIncrementPattern = /* lang=regex */ @"^[A-Za-z_]+\s?(\+\+|--|)$";
    /// <summary>
    /// name='abc'
    /// </summary>
    private const string StringSetPattern = /* lang=regex */ @"^[A-Za-z_]+\s?=\s?'[^']+'$";

    public string? Parse(XmlReader reader, IParsingContext parsingContext, string value)
    {
        value = value.Trim();

        if (Regex.IsMatch(value, IntegerSetPattern))
            return value;

        if (Regex.IsMatch(value, IntegerIncrementPattern))
            return value;

        if (Regex.IsMatch(value, StringSetPattern))
            return value;

        parsingContext.LogError(reader, "Valor de atribuição inválido.");
        return null;
    }
}
