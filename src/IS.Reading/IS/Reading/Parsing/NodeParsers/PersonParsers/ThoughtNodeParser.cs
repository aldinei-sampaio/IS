using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class ThoughtNodeParser : BalloonTextNodeParserBase, IThoughtNodeParser
{
    public ThoughtNodeParser(IElementParser elementParser, IThoughtChildNodeParser childParser)
        : base(elementParser, childParser)
    {
    }
}
