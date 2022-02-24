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
        => this.wordReaderFactory = wordReaderFactory;

    public Result<ICondition> Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result.Fail<ICondition>("Era esperado um argumento com uma condição.");

        var reader = wordReaderFactory.Create(text);
        return ReadRoot(reader, false, false);
    }

    private static Result<ICondition> ReadingError(IWordReader reader, string message)
        => Result.Fail<ICondition>(string.Format(message, reader.Word));

    private class ReadRootContext
    {
        public IWordReader Reader { get; }
        public bool AllowCloseParenthesys { get; }
        public bool SingleCondition { get; }

        public ReadRootContext(IWordReader reader, bool allowCloseParenthesys, bool singleCondition)
        {
            Reader = reader;
            SingleCondition = singleCondition;
            AllowCloseParenthesys = allowCloseParenthesys;
        }

        public Result<ICondition>? Current { get; set; }

        public bool ShouldReturnCurrent { get; set; }
    }

    private static Result<ICondition> ReadRoot(IWordReader reader, bool allowCloseParenthesys, bool singleCondition)
    {
        if (!reader.Read())
            return Result.Fail<ICondition>(UnexpectedExpressionEnd);

        var context = new ReadRootContext(reader, allowCloseParenthesys, singleCondition);

        do
        {
            switch (reader.WordType)
            {
                case WordType.Invalid:
                    return ReadingError(reader, InvalidToken);

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
                    return ReadingError(reader, MisplacedKeyword);
            }

            if (context.ShouldReturnCurrent)
                return context.Current!.Value;
        }
        while (reader.Read());

        if (context.AllowCloseParenthesys || !context.Current.HasValue)
            return Result.Fail<ICondition>(UnexpectedExpressionEnd);

        return context.Current.Value;
    }

    private static void ReadRootVariableOrConstant(ReadRootContext context)
    {
        if (context.Current is not null)
        {
            context.Current = ReadingError(context.Reader, MissingLogicalOperator);
            context.ShouldReturnCurrent = true;
            return;
        }

        context.Current = ReadCondition(context.Reader);

        if (context.SingleCondition || context.Current is null)
            context.ShouldReturnCurrent = true;
    }

    private static void ReadRootOpenParenthesys(ReadRootContext context)
    {
        if (context.Current is not null)
        {
            context.Current = ReadingError(context.Reader, MissingLogicalOperator);
            context.ShouldReturnCurrent = true;
            return;
        }

        context.Current = ReadRoot(context.Reader, true, false);
        context.ShouldReturnCurrent = context.Current is null;
    }

    private static void ReadRootCloseParenthesys(ReadRootContext context)
    {
        if (!context.AllowCloseParenthesys || context.Current is null)
            context.Current = ReadingError(context.Reader, MisplacedKeyword);

        context.ShouldReturnCurrent = true;
    }

    private static void ReadRootLogicalOperator(ReadRootContext context, Func<ICondition, ICondition, ICondition> factory)
    {
        if (context.Current is null)
        {
            context.Current = ReadingError(context.Reader, MisplacedKeyword);
            context.ShouldReturnCurrent = true;
            return;
        }

        var right = ReadRoot(context.Reader, context.AllowCloseParenthesys, true);
        if (!right.IsOk)
        {
            context.Current = right;
            context.ShouldReturnCurrent = true;
            return;
        }

        context.Current = Result.Ok(factory.Invoke(context.Current.Value.Value, right.Value));

        if (context.AllowCloseParenthesys && context.Reader.WordType == WordType.CloseParenthesys)
            context.ShouldReturnCurrent = true;
    }

    private static void ReadRootNot(ReadRootContext context)
    {
        if (context.Current is not null)
        {
            context.Current = ReadingError(context.Reader, MissingLogicalOperator);
            context.ShouldReturnCurrent = true;
            return;
        }

        var right = ReadRoot(context.Reader, false, true);
        if (!right.IsOk)
        {
            context.Current = right;
            context.ShouldReturnCurrent = true;
            return;
        }

        context.Current = Result.Ok<ICondition>(new NotCondition(right.Value));
    }

    private static Result<IConditionKeyword> ReadKeyword(IWordReader reader)
    {
        if (reader.WordType == WordType.Null)
            return Result.Ok<IConditionKeyword>(new ConstantCondition(null));

        if (reader.WordType == WordType.Variable)
            return Result.Ok<IConditionKeyword>(new VariableCondition(reader.Word));

        if (reader.WordType == WordType.String)
            return Result.Ok<IConditionKeyword>(new ConstantCondition(reader.Word));

        if (reader.WordType == WordType.Number)
        {
            if (int.TryParse(reader.Word, out var number))
                return Result.Ok<IConditionKeyword>(new ConstantCondition(number));

            return Result.Fail<IConditionKeyword>(string.Format(InvalidNumber, reader.Word));
        }

        return Result.Fail<IConditionKeyword>(string.Format(InvalidOperand, reader.Word));
    }

    private static Result<IConditionKeyword> ReadNextKeyword(IWordReader reader)
    {
        if (!reader.Read())
            return Result.Fail<IConditionKeyword>(UnexpectedExpressionEnd);

        if (reader.WordType == WordType.Invalid)
            return Result.Fail<IConditionKeyword>(string.Format(InvalidToken, reader.Word));

        return ReadKeyword(reader);
    }

    private static Result<ICondition> ReadCondition(IWordReader reader)
    {
        var result = ReadKeyword(reader);
        if (!result.IsOk)
            return Result.Fail<ICondition>(result.ErrorMessage);

        var left = result.Value;

        if (!reader.Read())
            return Result.Fail<ICondition>(UnexpectedExpressionEnd);

        return reader.WordType switch
        {
            WordType.Invalid => ReadingError(reader, InvalidToken),
            WordType.Equals => ReadOperator(reader, left, (l, r) => new EqualsToCondition(l, r)),
            WordType.NotEqualsTo => ReadOperator(reader, left, (l, r) => new NotEqualsToCondition(l, r)),
            WordType.GreaterThan => ReadOperator(reader, left, (l, r) => new GreaterThanCondition(l, r)),
            WordType.LowerThan => ReadOperator(reader, left, (l, r) => new LowerThanCondition(l, r)),
            WordType.EqualOrGreaterThan => ReadOperator(reader, left, (l, r) => new EqualOrGreaterThanCondition(l, r)),
            WordType.EqualOrLowerThan => ReadOperator(reader, left, (l, r) => new EqualOrLowerThanCondition(l, r)),
            WordType.In => ReadInCondition(left, false, reader),
            WordType.Between => ReadBetweenCondition(left, false, reader),
            WordType.Not => ReadNotOperator(reader, left),
            WordType.Is => ReadNullCondition(left, reader),
            _ => ReadingError(reader, InvalidOperator),
        };
    }

    private static Result<ICondition> ReadNotOperator(
        IWordReader reader,
        IConditionKeyword left
    )
    {
        var errorMessage = ReadExpectingNotBeTheEnd(reader);
        if (errorMessage is not null)
            return Result.Fail<ICondition>(errorMessage);

        if (reader.WordType == WordType.In)
            return ReadInCondition(left, true, reader);

        if (reader.WordType == WordType.Between)
            return ReadBetweenCondition(left, true, reader);

        return ReadingError(reader, InvalidTokenAfterNot);
    }

    private static Result<ICondition> ReadOperator(
        IWordReader reader, 
        IConditionKeyword left, 
        Func<IConditionKeyword, IConditionKeyword, ICondition> factory
    )
    {
        var right = ReadNextKeyword(reader);
        if (!right.IsOk)
            return Result.Fail<ICondition>(right.ErrorMessage);
        return Result.Ok(factory.Invoke(left, right.Value));
    }

    private static Result<ICondition> ReadNullCondition(IConditionKeyword operand, IWordReader reader)
    {
        var errorMessage = ReadExpectingNotBeTheEnd(reader);
        if (errorMessage is not null)
            return Result.Fail<ICondition>(errorMessage);

        if (reader.WordType == WordType.Null)
            return Result.Ok<ICondition>(new IsNullCondition(operand));

        if (reader.WordType != WordType.Not)
            return ReadingError(reader, InvalidTokenAfterIs);

        errorMessage = ReadExpectingNotBeTheEnd(reader);
        if (errorMessage is not null)
            return Result.Fail<ICondition>(errorMessage);

        if (reader.WordType == WordType.Null)
            return Result.Ok<ICondition>(new IsNotNullCondition(operand));

        return ReadingError(reader, InvalidTokenAfterIsNot);
    }

    private static Result<ICondition> ReadBetweenCondition(IConditionKeyword operand, bool negated, IWordReader reader)
    {
        var min = ReadNextKeyword(reader);
        if (!min.IsOk)
            return Result.Fail<ICondition>(min.ErrorMessage);

        var errorMessage = ReadExpectingNotBeTheEnd(reader);
        if (errorMessage is not null)
            return Result.Fail<ICondition>(errorMessage);

        if (reader.WordType != WordType.And)
            return ReadingError(reader, BetweenWithoutAnd);

        var max = ReadNextKeyword(reader);
        if (!max.IsOk)
            return Result.Fail<ICondition>(max.ErrorMessage);

        if (negated)
            return Result.Ok<ICondition>(new NotBetweenCondition(operand, min.Value, max.Value));

        return Result.Ok<ICondition>(new BetweenCondition(operand, min.Value, max.Value));
    }

    private static Result<ICondition> ReadInCondition(IConditionKeyword operand, bool negated, IWordReader reader)
    {
        var errorMessage = ReadExpectingNotBeTheEnd(reader);
        if (errorMessage is not null)
            return Result.Fail<ICondition>(errorMessage);

        if (reader.WordType != WordType.OpenParenthesys)
            return ReadingError(reader, InvalidTokenAfterIn);

        var values = new List<IConditionKeyword>();
        Result<IConditionKeyword> value;

        while (true)
        {
            value = ReadNextKeyword(reader);
            if (!value.IsOk)
                return Result.Fail<ICondition>(value.ErrorMessage);
            values.Add(value.Value);

            errorMessage = ReadExpectingNotBeTheEnd(reader);
            if (errorMessage is not null)
                return Result.Fail<ICondition>(errorMessage);

            if (reader.WordType == WordType.CloseParenthesys)
                break;

            if (reader.WordType != WordType.Comma)
                return ReadingError(reader, InvalidTokenAfterInValue);
        }

        if (negated)
            return Result.Ok<ICondition>(new NotInCondition(operand, values));

        return Result.Ok<ICondition>(new InCondition(operand, values));
    }

    private static string? ReadExpectingNotBeTheEnd(IWordReader reader)
    {
        if (!reader.Read())
            return UnexpectedExpressionEnd;

        if (reader.WordType == WordType.Invalid)
            return string.Format(InvalidToken, reader.Word);

        return null;
    }
}
