namespace IS.Reading.Parsing.ArgumentParsers;

public class IntegerArgumentParser : IIntegerArgumentParser
{
    public int? Parse(IDocumentReader reader, IParsingContext parsingContext, string value, int minValue, int maxValue)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!int.TryParse(value, out var parsed))
        { 
            parsingContext.LogError(reader, $"O texto '{value}' não representa um número inteiro válido.");
            return null;
        }

        if (parsed < minValue || parsed > maxValue)
        {
            parsingContext.LogError(reader, $"O valor precisa estar entre {minValue} e {maxValue}.");
            return null;
        }

        return parsed;
    }
}
