using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class TutorialTextNodeParser : GenericTextNodeParserBase, ITutorialTextNodeParser
{
    public TutorialTextNodeParser(IElementParser elementParser, IBalloonTextParser balloonTextParser) 
        : base(elementParser, balloonTextParser)
    {
    }

    public override string Name => "tutorial";

    public BalloonType BalloonType => BalloonType.Tutorial;
}
