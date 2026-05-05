namespace IS.Reading.Parsing.ArgumentParsers;

public interface IIntegerArgumentParser
{
    Result<int> Parse(string value, int minValue, int maxValue);
}

public class IntegerArgumentParser : IIntegerArgumentParser
{
    public Result<int> Parse(string value, int minValue, int maxValue)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail<int>($"Era esperado um argumento com um número inteiro entre {minValue} e {maxValue}.");

        if (!int.TryParse(value, out var parsed))
            return Result.Fail<int>($"O texto '{value}' não representa um número inteiro válido.");

        if (parsed < minValue || parsed > maxValue)
            return Result.Fail<int>($"O valor precisa estar entre {minValue} e {maxValue}.");

        return Result.Ok(parsed);
    }
}
