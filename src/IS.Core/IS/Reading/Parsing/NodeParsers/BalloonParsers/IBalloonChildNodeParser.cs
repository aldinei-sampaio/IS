namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public interface IBalloonChildNodeParser : IAggregateNodeParser
{
    BalloonType BalloonType { get; }
}
