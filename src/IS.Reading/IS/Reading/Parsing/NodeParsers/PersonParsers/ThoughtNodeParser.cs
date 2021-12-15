using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class ThoughtNodeParser : BalloonTextNodeParserBase, IThoughtNodeParser
{
    public ThoughtNodeParser(IElementParser elementParser, IThoughtChildNodeParser childParser, IMoodNodeParser moodNodeParser)
        : base(elementParser, childParser, moodNodeParser)
    {
    }
}
