namespace IS.Reading.Variables;

public class InterpolatorParser : ITextSourceParser
{
    public ITextSourceParserResult Parse(string text)
    {
        var charsToFind = new[] { '{', '}' };

        var n = text.IndexOfAny(charsToFind);
        if (n == -1)
            return new TextSourceParserResult(new TextSource(text));
        
        var list = new List<IInterpolatedValue>();

        var span = text.AsSpan();

        for (; ; )
        {
            if (span[n] == '}')
            {
                if (!IsPrefixed(span, n, '}'))
                    return new TextSourceParserResult("'}' sem um '{' correspondente.");

                list.Add(new InterpolatedValue("}", false));
                span = span[(n + 1)..];
            }
            else
            {
                if (IsPrefixed(span, n, '}'))
                {
                    list.Add(new InterpolatedValue("{", false));
                    span = span[(n + 2)..];
                }
                else
                {
                    var m = span.IndexOfAny(charsToFind);
                    if (m == -1 || span[m] == '{')
                        return new TextSourceParserResult("'{' sem um '}' correspondente.");

                    list.Add(new InterpolatedValue(span[(n + 1)..(m - 1)].ToString(), true));
                    span = span[(m + 1)..];
                }
            }

            if (span.Length == 0)
                break;

            n = span.IndexOfAny(charsToFind);
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
