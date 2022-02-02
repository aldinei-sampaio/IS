using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class BalloonChildParsingContext : IParentParsingContext
{
    public BalloonType BalloonType { get; }

    public BalloonChildParsingContext(BalloonType balloonType)
        => BalloonType = balloonType;

    public ChoiceNode? ChoiceNode { get; set; }
}
