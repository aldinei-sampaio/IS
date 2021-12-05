using IS.Reading.Navigation;

namespace IS.Reading.Parsing.NodeParsers;

public class BalloonTextParentParsingContext : IParentParsingContext
{
    public Block Block { get; } = new();
}