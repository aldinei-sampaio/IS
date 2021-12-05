using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class BalloonTextChildParentParsingContext : IParentParsingContext
{
    public ChoiceNode? ChoiceNode { get; set; }
}
