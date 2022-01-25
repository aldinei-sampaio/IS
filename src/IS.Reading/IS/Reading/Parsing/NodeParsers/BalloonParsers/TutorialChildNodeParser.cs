using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class TutorialChildNodeParser : BalloonChildNodeParserBase, ITutorialChildNodeParser
{
    public TutorialChildNodeParser(
        IElementParser elementParser, 
        ITextSourceParser textSourceParser,
        ITutorialTextNodeParser tutorialTextNodeParser, 
        IChoiceNodeParser choiceNodeParser
    ) 
    : base(elementParser, textSourceParser, tutorialTextNodeParser, choiceNodeParser)
    {
    }
}
