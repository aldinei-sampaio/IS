using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class SpeechNodeParser : BalloonTextNodeParserBase, ISpeechNodeParser
{
    public SpeechNodeParser(
        IElementParser elementParser, 
        ISpeechChildNodeParser childParser, 
        IMoodNodeParser moodNodeParser, 
        ISetNodeParser setNodeParser,
        IUnsetNodeParser unsetNodeParser
    ) 
        : base(elementParser, childParser, moodNodeParser, setNodeParser, unsetNodeParser)
    {
    }
}
