namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class SpeechChildNodeParser : BalloonChildNodeParserBase, ISpeechChildNodeParser
{
    public SpeechChildNodeParser(
        IElementParser elementParser, 
        ISpeechTextNodeParser speechTextNodeParser, 
        IChoiceNodeParser choiceNodeParser
    ) 
    : base(elementParser, speechTextNodeParser, choiceNodeParser)
    {
    }
}
