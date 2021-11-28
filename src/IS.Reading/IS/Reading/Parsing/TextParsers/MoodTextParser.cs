using System.Xml;

namespace IS.Reading.Parsing.TextParsers;

public class MoodTextParser : IMoodTextParser
{
    private readonly string[] validValues;

    public MoodTextParser()
        => validValues = Enum.GetNames(typeof(MoodType));

    public string? Parse(XmlReader reader, IParsingContext parsingContext, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.AsSpan().Trim();

        foreach(var validValue in validValues)
        {
            if (trimmed.CompareTo(validValue, StringComparison.OrdinalIgnoreCase) == 0)
                return validValue;
        }

        parsingContext.LogError(reader, $"O valor '{value}' não representa uma emoção válida.");
        return null;
    }
}
