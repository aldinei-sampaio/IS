using IS.Reading.Conditions;

namespace IS.Reading.Parsing;

public class TextParentParsingContext : IParentParsingContext
{
    public string? ParsedText { get; set; }
    public ICondition? When { get; set; }
}
