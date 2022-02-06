using System.Text.RegularExpressions;

namespace IS.Reading.Variables;

public class TextSourceParser : ITextSourceParser
{
    public const string MissingOpeningCurlyBraces = "'}' sem um '{' correspondente.";
    public const string MissingClosingCurlyBraces = "'{' sem um '}' correspondente.";
    public const string InvalidVariableName = "Nome de variável inválido: '{0}'";

    public Result<ITextSource> Parse(string text)
    {
        const char varPrefix = '{';
        const char varSuffix = '}';
        const string varPrefixString = "{";
        const string varSuffixString = "}";

        var span = text.AsSpan();

        var n = span.IndexOfAny(varPrefix, varSuffix);
        if (n == -1)
            return Result.Ok<ITextSource>(new TextSource(text));

        var list = new List<IInterpolatedValue>();

        for (; ; )
        {
            if (n > 0)
                list.Add(new InterpolatedValue(span[..n].ToString(), false));

            if (span[n] == varSuffix)
            {
                if (!IsPrefixed(span, n + 1, varSuffix))
                    return Result.Fail<ITextSource>(MissingOpeningCurlyBraces);

                list.Add(new InterpolatedValue(varSuffixString, false));
                span = span[(n + 2)..];
            }
            else
            {
                if (IsPrefixed(span, n + 1, varPrefix))
                {
                    list.Add(new InterpolatedValue(varPrefixString, false));
                    span = span[(n + 2)..];
                }
                else
                {
                    span = span[(n + 1)..];
                    var m = span.IndexOfAny(varPrefix, varSuffix);
                    if (m == -1 || span[m] == varPrefix)
                        return Result.Fail<ITextSource>(MissingClosingCurlyBraces);

                    if (m > 0)
                    {
                        var variableName = span[..m].ToString();
                        if (!Regex.IsMatch(variableName, "^[A-Za-z][A-Za-z0-9_]*$"))
                            return Result.Fail<ITextSource>(string.Format(InvalidVariableName, variableName));

                        list.Add(new InterpolatedValue(variableName, true));
                    }
                    span = span[(m + 1)..];
                }
            }

            if (span.Length == 0)
                break;

            n = span.IndexOfAny(varPrefix, varSuffix);
            if (n == -1)
            {
                list.Add(new InterpolatedValue(span.ToString(), false));
                break;
            }
        }

        if (list.Count == 0)
            return Result.Ok<ITextSource>(new TextSource(string.Empty));

        return Result.Ok<ITextSource>(new TextSource(new Interpolator(list, text.Length)));
    }

    private static bool IsPrefixed(ReadOnlySpan<char> span, int n, char c)
    {
        if (n >= span.Length)
            return false;

        return span[n] == c;
    }
}
