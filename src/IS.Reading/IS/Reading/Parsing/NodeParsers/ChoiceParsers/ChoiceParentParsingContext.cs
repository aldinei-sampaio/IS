using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceParentParsingContext : IParentParsingContext
{
    public ChoiceNode Choice { get; set; } = new();
}
