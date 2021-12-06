using IS.Reading.Navigation;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class BalloonTextParentParsingContext : IParentParsingContext
{
    public Block Block { get; } = new();
}