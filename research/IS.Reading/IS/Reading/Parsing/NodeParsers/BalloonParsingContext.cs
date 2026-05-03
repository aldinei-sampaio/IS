namespace IS.Reading.Parsing.NodeParsers;

public class BalloonParsingContext : ParentParsingContext
{
    public BalloonType BalloonType { get; }

    public BalloonParsingContext(BalloonType balloonType)
        => BalloonType = balloonType;
}
