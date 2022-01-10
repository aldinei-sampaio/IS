using IS.Reading.Conditions;

namespace IS.Reading.Parsing.ConditionParsers;

public class ConditionParser : IConditionParser
{
    internal const string UnexpectedExpressionEnd = "Fim inesperado da expressão.";
    internal const string MissingLogicalOperator = "É esperado um operador lógico (And/Or) ao invés de '{0}'.";
    internal const string MisplacedKeyword = "'{0}' não é válido nesse ponto da expressão.";
    internal const string InvalidNumber = "O valor '{0}' não é um número válido.";
    internal const string InvalidTokenAfterNot = "Após o 'Not' é esperado 'In' ou 'Between' ao invés de '{0}'.";
    internal const string InvalidTokenAfterIs = "Após o 'Is' é esperado 'Null' ou 'Not Null' ao invés de '{0}'.";
    internal const string InvalidTokenAfterIsNot = "Após o 'Is Not' é esperado 'Null' ao invés de '{0}'.";
    internal const string InvalidTokenAfterIn = "Após o 'In' é esperado '(' ao invés de '{0}'.";
    internal const string InvalidTokenAfterInValue = "Após um valor do 'In' é esperado ',' ou ')' ao invés de '{0}'.";
    internal const string InvalidToken = "A expressão '{0}' não representa uma palavra chave, valor ou operador válido.";
    internal const string InvalidOperand = "Era esperada uma variável ou uma constante ao invés de '{0}'.";
    internal const string InvalidOperator = "Era esperado um operador de comparação ao invés de '{0}'.";
    internal const string BetweenWithoutAnd = "Era esperado um 'And' entre os valores da cláusula 'Between', ao invés de '{0}'.";

    private readonly IWordReaderFactory wordReaderFactory;

    public ConditionParser(IWordReaderFactory wordReaderFactory)
    {
        this.wordReaderFactory = wordReaderFactory;
    }

    public IParsedCondition Parse(string text)
    {
        var reader = wordReaderFactory.Create(text);
        var messages = new List<string>();
        var condition = ReadRoot(reader, messages, false, false);
        return new ParsedCondition(condition, string.Join("\r\n", messages));
    }

    private static ICondition? ReadingError(IWordReader reader, List<string> messages, string message)
    {
        messages.Add(string.Format(message, reader.Word));
        return null;
    }

    private class ReadRootContext
    {
        public IWordReader Reader { get; }
        public List<string> Messages { get; }
        public bool AllowCloseParenthesys { get; }
        public bool SingleCondition { get; }

        public ReadRootContext(IWordReader reader, List<string> messages, bool allowCloseParenthesys, bool singleCondition)
        {
            Reader = reader;
            Messages = messages;
            SingleCondition = singleCondition;
            AllowCloseParenthesys = allowCloseParenthesys;
        }

        public ICondition? Current { get; set; }

        public bool ShouldReturnCurrent { get; set; }
    }

    private static ICondition? ReadRoot(IWordReader reader, List<string> messages, bool allowCloseParenthesys, bool singleCondition)
    {
        if (!reader.Read())
        {
            messages.Add(UnexpectedExpressionEnd);
            return null;
        }

        var context = new ReadRootContext(reader, messages, allowCloseParenthesys, singleCondition);

        do
        {
            switch (reader.WordType)
            {
                case WordType.Invalid:
                    return ReadingError(reader, messages, InvalidToken);

                case WordType.Variable:
                case WordType.String:
                case WordType.Number:
                case WordType.Null:
                    ReadRootVariableOrConstant(context);
                    break;

                case WordType.OpenParenthesys:
                    ReadRootOpenParenthesys(context);
                    break;

                case WordType.CloseParenthesys:
                    ReadRootCloseParenthesys(context);
                    break;

                case WordType.And:
                    ReadRootLogicalOperator(context, (l, r) => new AndCondition(l, r));
                    break;

                case WordType.Or:
                    ReadRootLogicalOperator(context, (l, r) => new OrCondition(l, r));
                    break;

                case WordType.Not:
                    ReadRootNot(context);
                    break;

                default:
                    return ReadingError(reader, messages, MisplacedKeyword);
            }

            if (context.ShouldReturnCurrent)
                return context.Current;
        }
        while (reader.Read());

        if (context.AllowCloseParenthesys || context.Current is null)
        {
            messages.Add(UnexpectedExpressionEnd);
            return null;
        }

        return context.Current;
    }

    private static void ReadRootVariableOrConstant(ReadRootContext context)
    {
        if (context.Current is not null)
        {
            context.Current = ReadingError(context.Reader, context.Messages, MissingLogicalOperator);
            context.ShouldReturnCurrent = true;
            return;
        }

        context.Current = ReadCondition(context.Reader, context.Messages);

        if (context.SingleCondition || context.Current is null)
            context.ShouldReturnCurrent = true;
    }

    private static void ReadRootOpenParenthesys(ReadRootContext context)
    {
        if (context.Current is not null)
        {
            context.Current = ReadingError(context.Reader, context.Messages, MissingLogicalOperator);
            context.ShouldReturnCurrent = true;
            return;
        }

        context.Current = ReadRoot(context.Reader, context.Messages, true, false);
        context.ShouldReturnCurrent = context.Current is null;
    }

    private static void ReadRootCloseParenthesys(ReadRootContext context)
    {
        if (!context.AllowCloseParenthesys || context.Current is null)
            context.Current = ReadingError(context.Reader, context.Messages, MisplacedKeyword);

        context.ShouldReturnCurrent = true;
    }

    private static void ReadRootLogicalOperator(ReadRootContext context, Func<ICondition, ICondition, ICondition> factory)
    {
        if (context.Current is null)
        {
            context.Current = ReadingError(context.Reader, context.Messages, MisplacedKeyword);
            context.ShouldReturnCurrent = true;
            return;
        }

        var right = ReadRoot(context.Reader, context.Messages, context.AllowCloseParenthesys, true);
        if (right is null)
        {
            context.Current = null;
            context.ShouldReturnCurrent = true;
            return;
        }

        context.Current = factory.Invoke(context.Current, right);

        if (context.AllowCloseParenthesys && context.Reader.WordType == WordType.CloseParenthesys)
            context.ShouldReturnCurrent = true;
    }

    private static void ReadRootNot(ReadRootContext context)
    {
        if (context.Current is not null)
        {
            context.Current = ReadingError(context.Reader, context.Messages, MissingLogicalOperator);
            context.ShouldReturnCurrent = true;
            return;
        }

        var right = ReadRoot(context.Reader, context.Messages, false, true);
        if (right is null)
        {
            context.Current = null;
            context.ShouldReturnCurrent = true;
            return;
        }

        context.Current = new NotCondition(right);
    }

    private static IConditionKeyword? ReadKeyword(IWordReader reader, List<string> messages)
    {
        if (reader.WordType == WordType.Null)
            return new ConstantCondition(null);

        if (reader.WordType == WordType.Variable)
            return new VariableCondition(reader.Word);

        if (reader.WordType == WordType.String)
            return new ConstantCondition(reader.Word);

        if (reader.WordType == WordType.Number)
        {
            if (int.TryParse(reader.Word, out var number))
                return new ConstantCondition(number);

            messages.Add(string.Format(InvalidNumber, reader.Word));
            return null;
        }

        messages.Add(string.Format(InvalidOperand, reader.Word));
        return null;
    }

    private static IConditionKeyword? ReadNextKeyword(IWordReader reader, List<string> messages)
    {
        if (!reader.Read())
        {
            messages.Add(UnexpectedExpressionEnd);
            return null;
        }

        if (reader.WordType == WordType.Invalid)
        {
            messages.Add(string.Format(InvalidToken, reader.Word));
            return null;
        }

        return ReadKeyword(reader, messages);
    }

    private static ICondition? ReadCondition(IWordReader reader, List<string> messages)
    {
        var left = ReadKeyword(reader, messages);
        if (left is null)
            return null;

        if (!reader.Read())
        {
            messages.Add(UnexpectedExpressionEnd);
            return null;
        }

        switch (reader.WordType)
        {
            case WordType.Invalid:
                messages.Add(string.Format(InvalidToken, reader.Word));
                return null;

            case WordType.Equals:
                return ReadOperator(reader, messages, left, (l, r) => new EqualsToCondition(l, r));

            case WordType.NotEqualsTo:
                return ReadOperator(reader, messages, left, (l, r) => new NotEqualsToCondition(l, r));

            case WordType.GreaterThan:
                return ReadOperator(reader, messages, left, (l, r) => new GreaterThanCondition(l, r));

            case WordType.LowerThan:
                return ReadOperator(reader, messages, left, (l, r) => new LowerThanCondition(l, r));

            case WordType.EqualOrGreaterThan:
                return ReadOperator(reader, messages, left, (l, r) => new EqualOrGreaterThanCondition(l, r));

            case WordType.EqualOrLowerThan:
                return ReadOperator(reader, messages, left, (l, r) => new EqualOrLowerThanCondition(l, r));

            case WordType.In:
                return ReadInCondition(left, false, reader, messages);

            case WordType.Between:
                return ReadBetweenCondition(left, false, reader, messages);

            case WordType.Not:
                return ReadNotOperator(reader, messages, left);

            case WordType.Is:
                return ReadNullCondition(left, reader, messages);

            default:
                messages.Add(string.Format(InvalidOperator, reader.Word));
                return null;
        }
    }

    private static ICondition? ReadNotOperator(
        IWordReader reader,
        List<string> messages,
        IConditionKeyword left
    )
    {
        if (!ReadExpectingNotBeTheEnd(reader, messages))
            return null;

        if (reader.WordType == WordType.In)
            return ReadInCondition(left, true, reader, messages);

        if (reader.WordType == WordType.Between)
            return ReadBetweenCondition(left, true, reader, messages);

        messages.Add(string.Format(InvalidTokenAfterNot, reader.Word));
        return null;
    }

    private static ICondition? ReadOperator(
        IWordReader reader, 
        List<string> messages, 
        IConditionKeyword left, 
        Func<IConditionKeyword, IConditionKeyword, ICondition> factory
    )
    {
        var right = ReadNextKeyword(reader, messages);
        if (right is null)
            return null;
        return factory.Invoke(left, right);
    }

    private static ICondition? ReadNullCondition(IConditionKeyword operand, IWordReader reader, List<string> messages)
    {
        if (!ReadExpectingNotBeTheEnd(reader, messages))
            return null;

        if (reader.WordType == WordType.Null)
            return new IsNullCondition(operand);

        if (reader.WordType != WordType.Not)
        {
            messages.Add(string.Format(InvalidTokenAfterIs, reader.Word));
            return null;
        }

        if (!ReadExpectingNotBeTheEnd(reader, messages))
            return null;

        if (reader.WordType == WordType.Null)
            return new IsNotNullCondition(operand);

        messages.Add(string.Format(InvalidTokenAfterIsNot, reader.Word));
        return null;
    }

    private static ICondition? ReadBetweenCondition(IConditionKeyword operand, bool negated, IWordReader reader, List<string> messages)
    {
        var min = ReadNextKeyword(reader, messages);
        if (min is null)
            return null;

        if (!ReadExpectingNotBeTheEnd(reader, messages))
            return null;

        if (reader.WordType != WordType.And)
        {
            messages.Add(string.Format(BetweenWithoutAnd, reader.Word));
            return null;
        }

        var max = ReadNextKeyword(reader, messages);
        if (max is null)
            return null;

        if (negated)
            return new NotBetweenCondition(operand, min, max);

        return new BetweenCondition(operand, min, max);
    }

    private static ICondition? ReadInCondition(IConditionKeyword operand, bool negated, IWordReader reader, List<string> messages)
    {
        if (!ReadExpectingNotBeTheEnd(reader, messages))
            return null;

        if (reader.WordType != WordType.OpenParenthesys)
        {
            messages.Add(string.Format(InvalidTokenAfterIn, reader.Word));
            return null;
        }

        var values = new List<IConditionKeyword>();
        IConditionKeyword? value;

        for (; ; )
        {
            value = ReadNextKeyword(reader, messages);
            if (value is null)
                return null;
            values.Add(value);

            if (!ReadExpectingNotBeTheEnd(reader, messages))
                return null;

            if (reader.WordType == WordType.CloseParenthesys)
                break;

            if (reader.WordType != WordType.Comma)
            {
                messages.Add(string.Format(InvalidTokenAfterInValue, reader.Word));
                return null;
            }
        }

        if (negated)
            return new NotInCondition(operand, values);

        return new InCondition(operand, values);
    }

    private static bool ReadExpectingNotBeTheEnd(IWordReader reader, List<string> messages)
    {
        if (!reader.Read())
        {
            messages.Add(UnexpectedExpressionEnd);
            return false;
        }

        if (reader.WordType == WordType.Invalid)
        {
            messages.Add(string.Format(InvalidToken, reader.Word));
            return false;
        }

        return true;
    }
}
