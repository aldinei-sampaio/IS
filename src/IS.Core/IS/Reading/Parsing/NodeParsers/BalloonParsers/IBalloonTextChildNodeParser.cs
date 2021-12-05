namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public interface IBalloonTextChildNodeParser : IAggregateNodeParser
{
    BalloonType BalloonType { get; }
}
