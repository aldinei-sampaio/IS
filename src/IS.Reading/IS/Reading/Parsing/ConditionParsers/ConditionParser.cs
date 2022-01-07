using IS.Reading.Conditions;

namespace IS.Reading.Parsing.ConditionParsers;

public class ConditionParser : IConditionParser
{
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
            messages.Add("Condição vazia.");
            return null;
        }

        ICondition? current = null;

        do
        {
            switch (reader.WordType)
            {
                case WordType.Variable:
                case WordType.String:
                case WordType.Number:
                    if (current is not null)
                    {
                        messages.Add("Era esperado um operador.");
                        return null;
                    }
                    current = ReadCondition(reader, messages);
                    if (singleCondition || current is null)
                        return current;
                    break;
                case WordType.OpenParenthesys:
                    if (current is not null)
                    {
                        messages.Add("Era esperado um operador.");
                        return null;
                    }
                    current = ReadRoot(reader, messages, true, false);
                    if (current is null)
                        return null;
                    break;
                case WordType.CloseParenthesys:
                    if (!allowCloseParenthesys || current is null)
                    {
                        messages.Add("Fecha parênteses não esperado.");
                        return null;
                    }
                    return current;
                case WordType.And:
                    {
                        if (current is null)
                        {
                            messages.Add("Operador 'And' não esperado.");
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
                            messages.Add("Operador 'Or' não esperado.");
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
                            messages.Add("Operador 'Not' não esperado.");
                            return null;
                        }
                        var right = ReadRoot(reader, messages, false, true);
                        if (right is null)
                            return null;
                        current = new NotCondition(right);
                        break;
                    }
                default:
                    messages.Add("Expressão inválida.");
                    return null;

            }
        }
        while (reader.Read());

        if (current is null)
        {
            messages.Add("Fim inesperado da expressão.");
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

            messages.Add("Número inválido.");
            return null;
        }

        messages.Add("Era esperada uma variável ou uma constante.");
        return null;
    }

    private static IConditionKeyword? ReadNextKeyword(IWordReader reader, List<string> messages)
    {
        if (!reader.Read())
        {
            messages.Add("Era esperada uma variável ou uma constante.");
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
            messages.Add("Era esperado um operador de comparação.");
            return null;
        }

        switch (reader.WordType)
        {
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

                    messages.Add("Era esperado In ou Between.");
                    return null;
                }
            case WordType.Is:
                return ReadNullCondition(left, reader, messages);

            default:
                messages.Add("Era esperado um operador de comparação.");
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
            messages.Add("Era esperado Null ou Not Null.");
            return null;
        }

        if (!ReadExpectingNotBeTheEnd(reader, messages))
            return null;

        if (reader.WordType == WordType.Null)
            return new IsNotNullCondition(operand);

        messages.Add("Era esperado Null.");
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
            messages.Add("Condição Between mal formada.");
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
            if (negated)
                messages.Add("Condição Not In mal formada.");
            else
                messages.Add("Condição In mal formada.");
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
                messages.Add("Condição In mal formada.");
                return null;
            }
        }

        if (negated)
            return new NotInCondition(operand, values);

        return new InCondition(operand, values);
    }

    private static bool ReadExpectingNotBeTheEnd(IWordReader reader, List<string> messages)
    {
        if (reader.Read())
            return true;
        messages.Add("Fim inesperado da expressão.");
        return false;
    }
}
