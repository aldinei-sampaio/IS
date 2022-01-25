using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class NarrationChildNodeParser : BalloonChildNodeParserBase, INarrationChildNodeParser
{
    public NarrationChildNodeParser(
        IElementParser elementParser,
        ITextSourceParser textSourceParser,
        INarrationTextNodeParser narrationTextNodeParser,
        IChoiceNodeParser choiceNodeParser
    )
    : base(elementParser, textSourceParser, narrationTextNodeParser, choiceNodeParser)
    {
    }
}
