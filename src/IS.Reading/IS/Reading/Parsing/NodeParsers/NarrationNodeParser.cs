using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class NarrationNodeParser : BalloonTextNodeParserBase, INarrationNodeParser
{
    public NarrationNodeParser(IElementParser elementParser, INarrationChildNodeParser childParser)
        : base(elementParser, childParser)
    {        
    }
}
