namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public interface IBalloonTextNodeParser : INodeParser
{
    BalloonType BalloonType { get; }
}
