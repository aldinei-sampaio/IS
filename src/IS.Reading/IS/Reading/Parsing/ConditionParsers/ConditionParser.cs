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

    private static ICondition? ReadRoot(IWordReader reader, List<string> messages, bool allowCloseParenthesys, bool singleCondition)
    {
        if (!reader.Read())
        {
            messages.Add(UnexpectedExpressionEnd);
            return null;
        }

        ICondition? current = null;

        do
        {
            switch (reader.WordType)
            {
                case WordType.Invalid:
                    messages.Add(string.Format(InvalidToken, reader.Word));
                    return null;
                case WordType.Variable:
                case WordType.String:
                case WordType.Number:
                case WordType.Null:
                    if (current is not null)
                    {
                        messages.Add(string.Format(MissingLogicalOperator, reader.Word));
                        return null;
                    }
                    current = ReadCondition(reader, messages);
                    if (singleCondition || current is null)
                        return current;
                    break;
                case WordType.OpenParenthesys:
                    if (current is not null)
                    {
                        messages.Add(string.Format(MissingLogicalOperator, reader.Word));
                        return null;
                    }
                    current = ReadRoot(reader, messages, true, false);
                    if (current is null)
                        return null;
                    break;
                case WordType.CloseParenthesys:
                    if (!allowCloseParenthesys || current is null)
                    {
                        messages.Add(string.Format(MisplacedKeyword, reader.Word));
                        return null;
                    }
                    return current;
                case WordType.And:
                    {
                        if (current is null)
                        {
                            messages.Add(string.Format(MisplacedKeyword, reader.Word));
                            return null;
                        }
                        var right = ReadRoot(reader, messages, allowCloseParenthesys, true);
                        if (right is null)
                            return null;
                        current = new AndCondition(current, right);
                        if (allowCloseParenthesys && reader.WordType == WordType.CloseParenthesys)
                            return current;
                        break;
                    }
                case WordType.Or:
                    {
                        if (current is null)
                        {
                            messages.Add(string.Format(MisplacedKeyword, reader.Word));
                            return null;
                        }
                        var right = ReadRoot(reader, messages, allowCloseParenthesys, true);
                        if (right is null)
                            return null;
                        current = new OrCondition(current, right);
                        if (allowCloseParenthesys && reader.WordType == WordType.CloseParenthesys)
                            return current;
                        break;
                    }
                case WordType.Not:
                    {
                        if (current is not null)
                        {
                            messages.Add(string.Format(MissingLogicalOperator, reader.Word));
                            return null;
                        }
                        var right = ReadRoot(reader, messages, false, true);
                        if (right is null)
                            return null;
                        current = new NotCondition(right);
                        break;
                    }
                default:
                    messages.Add(string.Format(MisplacedKeyword, reader.Word));
                    return null;
            }
        }
        while (reader.Read());

        if (allowCloseParenthesys || current is null)
        {
            messages.Add(UnexpectedExpressionEnd);
            return null;
        }

        return current;
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
                {
                    var right = ReadNextKeyword(reader, messages);
                    if (right is null)
                        return null;
                    return new EqualsToCondition(left, right);
                }
            case WordType.NotEqualsTo:
                {
                    var right = ReadNextKeyword(reader, messages);
                    if (right is null)
                        return null;
                    return new NotEqualsToCondition(left, right);
                }
            case WordType.GreaterThan:
                {
                    var right = ReadNextKeyword(reader, messages);
                    if (right is null)
                        return null;
                    return new GreaterThanCondition(left, right);
                }
            case WordType.LowerThan:
                {
                    var right = ReadNextKeyword(reader, messages);
                    if (right is null)
                        return null;
                    return new LowerThanCondition(left, right);
                }
            case WordType.EqualOrGreaterThan:
                {
                    var right = ReadNextKeyword(reader, messages);
                    if (right is null)
                        return null;
                    return new EqualOrGreaterThanCondition(left, right);
                }
            case WordType.EqualOrLowerThan:
                {
                    var right = ReadNextKeyword(reader, messages);
                    if (right is null)
                        return null;
                    return new EqualOrLowerThanCondition(left, right);
                }
            case WordType.In:
                return ReadInCondition(left, false, reader, messages);

            case WordType.Between:
                return ReadBetweenCondition(left, false, reader, messages);

            case WordType.Not:
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
            case WordType.Is:
                return ReadNullCondition(left, reader, messages);

            default:
                messages.Add(string.Format(InvalidOperator, reader.Word));
                return null;
        }
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
