using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class NarrationNodeParser : Helpers.BalloonTextNodeParserBase, INarrationNodeParser
{
    public NarrationNodeParser(IElementParser elementParser, IBalloonTextParser balloonTextParser)
        : base("narration", BalloonType.Narration, elementParser, balloonTextParser)
    {        
    }
}
