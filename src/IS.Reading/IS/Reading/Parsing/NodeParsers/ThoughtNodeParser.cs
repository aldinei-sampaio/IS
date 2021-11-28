using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class ThoughtNodeParser : Helpers.BalloonTextNodeParserBase, IThoughtNodeParser
{
    public ThoughtNodeParser(IElementParser elementParser, IBalloonTextParser balloonTextParser)
        : base("thought", BalloonType.Thought, elementParser, balloonTextParser)
    {        
    }
}
