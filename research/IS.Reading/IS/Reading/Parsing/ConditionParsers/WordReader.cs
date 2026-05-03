namespace IS.Reading.Parsing.ConditionParsers;

public class WordReader : IWordReader
{
    public string Text { get; }

    private int currentPosition = 0;

    public WordReader(string text)
    {
        this.Text = text;
    }

    public WordType WordType { get; private set; } = WordType.None;

    public string Word { get; private set; } = string.Empty;

    public bool AtEnd => WordType == WordType.Invalid || currentPosition >= Text.Length;

    public bool Read()
    {
        Word = string.Empty;
        WordType = WordType.None;

        if (!PrepareForReading())
            return false;

        var span = Text.AsSpan(currentPosition);

        switch (span[0])
        {
            case '=':
                return ReadSingleCharToken(span, WordType.Equals);
            case '!':
                return ReadDifferent(span);
            case '<':
                return ReadLowerThan(span);
            case '>':
                return ReadGreaterThan(span);
            case '(':
                return ReadSingleCharToken(span, WordType.OpenParenthesys);
            case ')':
                return ReadSingleCharToken(span, WordType.CloseParenthesys);
            case '-':
            case >= '0' and <= '9':
                return ReadNumber(span);
            case '\'':
                return ReadString(span);
            case ',':
                return ReadSingleCharToken(span, WordType.Comma);
            case >= 'a' and <= 'z':
            case >= 'A' and <= 'Z':
            case '_':
                return ReadVariableOrKeyword(span);
            default:
                return ReadSingleCharToken(span, WordType.Invalid);
        }
    }

    private bool PrepareForReading()
    {
        if (AtEnd)
            return false;

        while (Text[currentPosition] == ' ')
        {
            currentPosition++;
            if (currentPosition >= Text.Length)
                return false;
        }
        return true;
    }

    private bool ReadSingleCharToken(ReadOnlySpan<char> span, WordType wordType)
    {
        currentPosition++;
        WordType = wordType;
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
            case '=':
                currentPosition++;
                WordType = WordType.EqualOrGreaterThan;
                Word = span[0..1].ToString();
                return true;
            case '_':
            case '\'':
            case ' ':
            case >= '0' and <= '9':
            case >= 'a' and <= 'z':
            case >= 'A' and <= 'Z':
                Word = span[0].ToString();
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
            case '=':
                currentPosition++;
                WordType = WordType.EqualOrLowerThan;
                Word = span[0..1].ToString();
                return true;
            case '>':
                currentPosition++;
                WordType = WordType.NotEqualsTo;
                Word = span[0..1].ToString();
                return true;
            case '_':
            case '\'':
            case ' ':
            case >= '0' and <= '9':
            case >= 'a' and <= 'z':
            case >= 'A' and <= 'Z':
                Word = span[0].ToString();
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
            WordType = WordType.NotEqualsTo;
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
                case ',':
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

    private bool ReadVariableOrKeyword(ReadOnlySpan<char> span)
    {
        currentPosition++;
        WordType = WordType.Variable;

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
                case ',':
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

    private bool ReadKeyWord(ReadOnlySpan<char> span, WordType wordType)
    {
        WordType = wordType;
        Word = span.ToString();
        return true;
    }

    private bool CheckForKeyWords(ReadOnlySpan<char> span)
    {
        switch (span.Length)
        {
            case 2:
                if (CheckFor2CharKeyWords(span))
                    return true;

                break;
            case 3:
                if (CheckFor3CharKeyWords(span))
                    return true;

                break;
            case 4:
                if (CheckFor4CharKeyWords(span))
                    return true;

                break;
            case 7:
                if (CheckFor7CharKeyWords(span))
                    return true;

                break;
        }

        Word = span.ToString();
        return true;
    }

    private bool CheckFor2CharKeyWords(ReadOnlySpan<char> span)
    {
        if (span[0] == 'o' && span[1] == 'r')
            return ReadKeyWord(span, WordType.Or);

        if (span[0] != 'i')
            return false;

        if (span[1] == 'n')
            return ReadKeyWord(span, WordType.In);

        if (span[1] == 's')
            return ReadKeyWord(span, WordType.Is);

        return false;
    }

    private bool CheckFor3CharKeyWords(ReadOnlySpan<char> span)
    {
        if (span[0] == 'a' && span[1] == 'n' && span[2] == 'd')
            return ReadKeyWord(span, WordType.And);

        if (span[0] == 'n' && span[1] == 'o' && span[2] == 't')
            return ReadKeyWord(span, WordType.Not);

        return false;
    }

    private bool CheckFor4CharKeyWords(ReadOnlySpan<char> span)
    {
        if (span[0] == 'n' &&
                   span[1] == 'u' &&
                   span[2] == 'l' &&
                   span[3] == 'l')
            return ReadKeyWord(span, WordType.Null);

        return false;
    }

    private bool CheckFor7CharKeyWords(ReadOnlySpan<char> span)
    {
        if (span[0] == 'b' &&
                    span[1] == 'e' &&
                    span[2] == 't' &&
                    span[3] == 'w' &&
                    span[4] == 'e' &&
                    span[5] == 'e' &&
                    span[6] == 'n')
            return ReadKeyWord(span, WordType.Between);

        return false;
    }
}