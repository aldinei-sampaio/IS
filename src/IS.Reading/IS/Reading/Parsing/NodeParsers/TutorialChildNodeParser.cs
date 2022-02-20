using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class TutorialChildNodeParser : DialogChildNodeParserBase, ITutorialChildNodeParser
{
    public TutorialChildNodeParser(
        IElementParser elementParser,
        ITextSourceParser textSourceParser,
        IChoiceNodeParser choiceNodeParser)
        : base(elementParser, textSourceParser, choiceNodeParser)
    {
    }

    override public string Name => "!";
}