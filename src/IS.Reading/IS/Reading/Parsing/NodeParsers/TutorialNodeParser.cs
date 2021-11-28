using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class TutorialNodeParser : Helpers.BalloonTextNodeParserBase, ITutorialNodeParser
{
    public TutorialNodeParser(IElementParser elementParser, IBalloonTextParser balloonTextParser)
        : base("tutorial", BalloonType.Tutorial, elementParser, balloonTextParser)
    {        
    }
}
