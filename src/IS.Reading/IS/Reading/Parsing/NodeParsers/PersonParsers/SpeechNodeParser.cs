using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class SpeechNodeParser : BalloonTextNodeParserBase, ISpeechNodeParser
{
    public SpeechNodeParser(IElementParser elementParser, ISpeechChildNodeParser childParser) 
        : base(elementParser, childParser)
    {
    }
}
