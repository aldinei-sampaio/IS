using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class SpeechChildNodeParser : DialogChildNodeParserBase, ISpeechChildNodeParser
{
    public SpeechChildNodeParser(
        IElementParser elementParser, 
        ITextSourceParser textSourceParser, 
        IChoiceNodeParser choiceNodeParser) 
        : base(elementParser, textSourceParser, choiceNodeParser)
    {
    }

    override public string Name => "-";
}