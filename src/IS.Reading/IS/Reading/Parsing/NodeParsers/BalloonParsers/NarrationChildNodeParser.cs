using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class NarrationChildNodeParser : BalloonTextChildNodeParserBase, INarrationChildNodeParser
{
    public NarrationChildNodeParser(
        IElementParser elementParser, 
        IBalloonTextParser balloonTextParser, 
        IChoiceNodeParser choiceNodeParser
    ) 
    : base(elementParser, balloonTextParser, choiceNodeParser)
    {
    }

    public override string Name => "narration";

    public override BalloonType BalloonType => BalloonType.Narration;
}
