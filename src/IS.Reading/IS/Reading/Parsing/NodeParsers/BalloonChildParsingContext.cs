using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers;

public class BalloonChildParsingContext : BalloonParsingContext
{
    public BalloonChildParsingContext(BalloonType balloonType) : base(balloonType)
    {
    }

    public IChoiceNode? ChoiceNode { get; set; }
}