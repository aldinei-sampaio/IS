using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class TutorialChildNodeParser : BalloonTextChildNodeParserBase, ITutorialChildNodeParser
{
    public TutorialChildNodeParser(
        IElementParser elementParser, 
        IBalloonTextParser balloonTextParser, 
        IChoiceNodeParser choiceNodeParser
    ) 
    : base(elementParser, balloonTextParser, choiceNodeParser)
    {
    }

    public override string Name => "tutorial";

    public override BalloonType BalloonType => BalloonType.Tutorial;
}
