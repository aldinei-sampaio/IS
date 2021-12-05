using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public class ParentParsingContext : IParentParsingContext
{
    public static ParentParsingContext Empty { get; } = new();
    private ParentParsingContext()
    {
    }
}

public class TextParentParsingContext : IParentParsingContext
{
    public string? ParsedText { get; set; }
    public ICondition? When { get; set; }
    public ICondition? While { get; set; }
}
