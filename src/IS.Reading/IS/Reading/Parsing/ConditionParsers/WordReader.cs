namespace IS.Reading.Parsing.ConditionParsers;

public class WordReader
{
    private readonly string text;
    private int currentPosition = 0;

    public WordReader(string text)
    {
        this.text = text;
    }

    public WordType WordType { get; private set; } = WordType.None;

    public string Word { get; private set; } = string.Empty;

    public bool AtEnd => WordType == WordType.Invalid || currentPosition >= text.Length;

    public bool Read()
    {
        Word = string.Empty;
        WordType = WordType.None;

        if (AtEnd)
            return false;

        while (text[currentPosition] == ' ')
        {
            currentPosition++;
            if (currentPosition >= text.Length)
                return false;
        }

        var span = text.AsSpan(currentPosition);

        switch (span[0])
        {
            case '=':
                currentPosition++;
                WordType = WordType.Equals;
                Word = span[0].ToString();
                return true;
            case '!':
                return ReadDifferent(span);
            case '<':
                return ReadLowerThan(span);
            case '>':
                return ReadGreaterThan(span);
            case '(':
                currentPosition++;
                WordType = WordType.OpenParenthesys;
                Word = span[0].ToString();
                return true;
            case ')':
                currentPosition++;
                WordType = WordType.CloseParenthesys;
                Word = span[0].ToString();
                return true;
            case '-':
            case >= '0' and <= '9':
                return ReadNumber(span);
            case '\'':
                return ReadString(span);
            case >= 'a' and <= 'z':
            case >= 'A' and <= 'Z':
            case '_':
                return ReadIdentifier(span);
        }

        WordType = WordType.Invalid;
        Word = span[0].ToString();
        return true;
    }

    private bool ReadString(ReadOnlySpan<char> span)
    {
        currentPosition++;
        WordType = WordType.String;
        for (var n = 1; n < span.Length; n++)
        {
            currentPosition++;
            if (span[n] == '\'')
            {
                if (n == 1)
                    Word = string.Empty;
                else
                    Word = span[1..n].ToString();
                return true;
            }
        }
        WordType = WordType.Invalid;
        Word = span.ToString();
        return true;
    }

    private bool ReadGreaterThan(ReadOnlySpan<char> span)
    {
        currentPosition++;
        WordType = WordType.GreaterThan;

        if (span.Length == 1)
        {
            Word = span[0].ToString();
            return true;
        }

        switch (span[1])
        {
            case >= '0' and <= '9':
            case >= 'a' and <= 'z':
            case >= 'A' and <= 'Z':
            case '_':
            case ' ':
                Word = span[0].ToString();
                return true;
            case '=':
                currentPosition++;
                WordType = WordType.EqualOrGreaterThan;
                Word = span[0..1].ToString();
                return true;
            default:
                WordType = WordType.Invalid;
                Word = span[0..1].ToString();
                return true;
        }
    }

    private bool ReadLowerThan(ReadOnlySpan<char> span)
    {
        currentPosition++;
        WordType = WordType.LowerThan;

        if (span.Length == 1)
        {
            Word = span[0].ToString();
            return true;
        }

        switch (span[1])
        {
            case >= '0' and <= '9':
            case >= 'a' and <= 'z':
            case >= 'A' and <= 'Z':
            case '_':
            case ' ':
                Word = span[0].ToString();
                return true;
            case '=':
                currentPosition++;
                WordType = WordType.EqualOrLowerThan;
                Word = span[0..1].ToString();
                return true;
            case '>':
                currentPosition++;
                WordType = WordType.Different;
                Word = span[0..1].ToString();
                return true;
            case ',':
                currentPosition++;
                WordType = WordType.Comma;
                Word = span[0..1].ToString();
                return true;
            default:
                WordType = WordType.Invalid;
                Word = span[0..1].ToString();
                return true;
        }
    }

    private bool ReadDifferent(ReadOnlySpan<char> span)
    {
        if (span.Length > 1 && span[1] == '=')
        {
            currentPosition += 2;
            WordType = WordType.Different;
            Word = span[0..1].ToString();
            return true;
        }
        WordType = WordType.Invalid;
        Word = span[0].ToString();
        return true;
    }

    private bool ReadNumber(ReadOnlySpan<char> span)
    {
        currentPosition++;
        WordType = WordType.Number;

        for (var n = 1; n < span.Length; n++)
        {
            switch (span[n])
            {
                case >= '0' and <= '9':
                    currentPosition++;
                    continue;
                case ' ':
                case '>':
                case '<':
                case '=':
                case '!':
                case ')':
                    Word = span[0..n].ToString();
                    return true;
                default:
                    WordType = WordType.Invalid;
                    Word = span[0..n].ToString();
                    return true;
            }
        }
        currentPosition++;
        Word = span.ToString();
        return true;
    }

    private bool ReadIdentifier(ReadOnlySpan<char> span)
    {
        currentPosition++;
        WordType = WordType.Identifier;

        for (var n = 1; n < span.Length; n++)
        {
            switch (span[n])
            {
                case >= 'a' and <= 'z':
                case >= 'A' and <= 'Z':
                case >= '0' and <= '9':
                case '_':
                    currentPosition++;
                    continue;
                case ' ':
                case '>':
                case '<':
                case '=':
                case '!':
                case ')':
                    Span<char> destination = new char[n];
                    span[0..n].ToLowerInvariant(destination);
                    return CheckForKeyWords(destination);
                default:
                    WordType = WordType.Invalid;
                    Word = span[0..n].ToString();
                    return true;
            }
        }
        {
            currentPosition++;
            Span<char> destination = new char[span.Length];
            span.ToLowerInvariant(destination);
            return CheckForKeyWords(destination);
        }
    }

    private bool CheckForKeyWords(ReadOnlySpan<char> span)
    {
        if (span.Length == 3)
        {
            if (span[0] == 'o' &&
                span[1] == 'r')
            {
                WordType = WordType.Or;
                Word = span.ToString();
                return true;
            }

            if (span[0] == 'i')
            {
                if (span[1] == 'n')
                {
                    WordType = WordType.In;
                    Word = span.ToString();
                    return true;
                }
                if (span[1] == 's')
                {
                    WordType = WordType.Is;
                    Word = span.ToString();
                    return true;
                }
            }
        }

        if (span.Length == 3)
        {
            if (span[0] == 'a' && 
                span[1] == 'n' && 
                span[2] == 'd')
            {
                WordType = WordType.And;
                Word = span.ToString();
                return true;
            }

            if (span[0] == 'n' && 
                span[1] == 'o' && 
                span[2] == 't')
            {
                WordType = WordType.Not;
                Word = span.ToString();
                return true;
            }
        }

        if (span.Length == 4 &&
            span[0] == 'n' &&
            span[1] == 'u' &&
            span[2] == 'l' &&
            span[3] == 'l')
        {
            WordType = WordType.Null;
            Word = span.ToString();
            return true;
        }

        if (span.Length == 7 &&
            span[0] == 'b' &&
            span[1] == 'e' &&
            span[2] == 't' &&
            span[3] == 'w' &&
            span[4] == 'e' &&
            span[5] == 'e' &&
            span[6] == 'n')
        {
            WordType = WordType.Between;
            Word = span.ToString();
            return true;
        }

        Word = span.ToString();
        return true;
    }
}