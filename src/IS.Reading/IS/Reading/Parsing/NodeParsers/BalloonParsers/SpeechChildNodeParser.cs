using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class SpeechChildNodeParser : BalloonChildNodeParserBase, ISpeechChildNodeParser
{
    public SpeechChildNodeParser(
        IElementParser elementParser,
        ITextSourceParser textSourceParser,
        ISpeechTextNodeParser speechTextNodeParser, 
        IChoiceNodeParser choiceNodeParser
    ) 
    : base(elementParser, textSourceParser, speechTextNodeParser, choiceNodeParser)
    {
    }
}
