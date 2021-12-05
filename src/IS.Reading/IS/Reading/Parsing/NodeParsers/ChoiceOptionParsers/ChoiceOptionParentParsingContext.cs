using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public class ChoiceOptionParentParsingContext : IParentParsingContext
{
    public ChoiceOptionNode Option { get; set; } = new();
    public string? ParsedText { get; set; }
}
