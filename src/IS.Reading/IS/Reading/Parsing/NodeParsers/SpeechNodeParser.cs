using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class SpeechNodeParser : Helpers.BalloonTextNodeParserBase, ISpeechNodeParser
{
    public SpeechNodeParser(IElementParser elementParser, IBalloonTextParser balloonTextParser)
        : base("speech", BalloonType.Speech, elementParser, balloonTextParser)
    {        
    }
}
