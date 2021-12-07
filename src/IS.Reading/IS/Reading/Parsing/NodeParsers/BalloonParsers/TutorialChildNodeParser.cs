namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class TutorialChildNodeParser : BalloonChildNodeParserBase, ITutorialChildNodeParser
{
    public TutorialChildNodeParser(
        IElementParser elementParser, 
        ITutorialTextNodeParser tutorialTextNodeParser, 
        IChoiceNodeParser choiceNodeParser
    ) 
    : base(elementParser, tutorialTextNodeParser, choiceNodeParser)
    {
    }
}
