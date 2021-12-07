using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class BalloonChildParsingContext : IParentParsingContext
{
    public string? ParsedText { get; set; }
    public ChoiceNode? ChoiceNode { get; set; }
}
