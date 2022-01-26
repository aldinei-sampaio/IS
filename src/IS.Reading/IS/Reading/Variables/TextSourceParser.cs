namespace IS.Reading.Variables;

public class TextSourceParser : ITextSourceParser
{
    public ITextSourceParserResult Parse(string text)
    {
        const char varPrefix = '{';
        const char varSuffix = '}';
        const string varPrefixString = "{";
        const string varSuffixString = "}";

        var span = text.AsSpan();

        var n = span.IndexOfAny(varPrefix, varSuffix);
        if (n == -1)
            return new TextSourceParserResult(new TextSource(text));
        
        var list = new List<IInterpolatedValue>();

        for (; ; )
        {
            if (n > 0)
                list.Add(new InterpolatedValue(span[..n].ToString(), false));

            if (span[n] == varSuffix)
            {
                if (!IsPrefixed(span, n + 1, varSuffix))
                    return new TextSourceParserResult("'}' sem um '{' correspondente.");

                list.Add(new InterpolatedValue(varSuffixString, false));
                span = span[(n + 1)..];
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
                        return new TextSourceParserResult("'{' sem um '}' correspondente.");

                    list.Add(new InterpolatedValue(span[..m].ToString(), true));
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

        return new TextSourceParserResult(new TextSource(new Interpolator(list, text.Length)));
    }

    private static bool IsPrefixed(ReadOnlySpan<char> span, int n, char c)
    { 
        if (n >= (span.Length - 1))
            return false;

        return span[n] == c;
    }
}
