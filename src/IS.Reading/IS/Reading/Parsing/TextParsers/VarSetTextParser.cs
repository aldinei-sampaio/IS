using System.Text.RegularExpressions;
using System.Xml;

namespace IS.Reading.Parsing.TextParsers;

public class VarSetTextParser : IVarSetTextParser
{
    /// <summary>
    /// name=1
    /// name+=1
    /// name-=1
    /// O número precisa ser um int32 (de -2147483648 a 2147483647)
    /// </summary>
    private const string IntegerSetPattern = /* lang=regex */ @"^\s*[A-Za-z_]+\s*(=|\+=|-=)\s*((-?([0-1]?\d{1,9}|2(0\d{8}|1[0-3]\d{7}|14[0-6]\d{6}|147[0-3]\d{5}|1474[0-7]\d{4}|14748[0-2]\d{3}|147483[0-5]\d{2}|1474836[0-3]\d|14748364[0-7])))|-2147483648)\s*$";
    /// <summary>
    /// name++
    /// name--
    /// </summary>
    private const string IntegerIncrementPattern = /* lang=regex */ @"^\s*[A-Za-z_]+\s*(\+\+|--)\s*$";
    /// <summary>
    /// name='abc'
    /// </summary>
    private const string StringSetPattern = /* lang=regex */ @"^\s*[A-Za-z_]+\s*=\s*'[^']+'\s*$";

    public string? Parse(XmlReader reader, IParsingContext parsingContext, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            if (Regex.IsMatch(value, IntegerSetPattern))
                return value;

            if (Regex.IsMatch(value, IntegerIncrementPattern))
                return value;

            if (Regex.IsMatch(value, StringSetPattern))
                return value;
        }

        parsingContext.LogError(reader, "Expressão 'Set' inválida.");
        return null;
    }
}
