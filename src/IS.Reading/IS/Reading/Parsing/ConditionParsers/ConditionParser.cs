using IS.Reading.Conditions;

namespace IS.Reading.Parsing.ConditionParsers;

public class ConditionParser : IConditionParser
{
    public IParsedCondition Parse(string text)
    {
        var reader = new WordReader(text);
        var messages = new List<string>();
        var condition = ReadRoot(reader, messages);
        return new ParsedCondition(condition, string.Join("\r\n", messages));
    }

    private static ICondition? ReadRoot(WordReader reader, List<string> messages)
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
                case WordType.Identifier:
                case WordType.String:
                case WordType.Number:
                    if (current is not null)
                    {
                        messages.Add("Era esperado um operador.");
                        return null;
                    }
                    current = ReadCondition(reader, messages);
                    if (current is null)
                        return null;
                    break;
                case WordType.OpenParenthesys:
                    if (current is not null)
                    {
                        messages.Add("Era esperado um operador.");
                        return null;
                    }
                    current = ReadRoot(reader, messages);
                    if (current is null)
                        return null;
                    break;
                case WordType.CloseParenthesys:
                    if (current is null)
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
                        var right = ReadRoot(reader, messages);
                        if (right is null)
                            return null;
                        current = new AndCondition(current, right);
                        break;
                    }
                case WordType.Or:
                    {
                        if (current is null)
                        {
                            messages.Add("Operador 'Or' não esperado.");
                            return null;
                        }
                        var right = ReadRoot(reader, messages);
                        if (right is null)
                            return null;
                        current = new OrCondition(current, right);
                        break;
                    }
                case WordType.Not:
                    {
                        if (current is not null)
                        {
                            messages.Add("Operador 'Not' não esperado.");
                            return null;
                        }
                        var right = ReadRoot(reader, messages);
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
        while (!reader.AtEnd);

        if (current is null)
        {
            messages.Add("Fim inesperado da expressão.");
            return null;
        }

        return current;
    }

    private static IConditionKeyword? ReadKeyword(WordReader reader, List<string> messages)
    {
        if (reader.WordType == WordType.Identifier)
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

    private static IConditionKeyword? ReadNextKeyword(WordReader reader, List<string> messages)
    {
        if (!reader.Read())
        {
            messages.Add("Era esperada uma variável ou uma constante.");
            return null;
        }
        return ReadKeyword(reader, messages);
    }

    private static ICondition? ReadCondition(WordReader reader, List<string> messages)
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
            case WordType.Different:
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
            default:
                messages.Add("Era esperado um operador de comparação.");
                return null;
        }
    }
}
