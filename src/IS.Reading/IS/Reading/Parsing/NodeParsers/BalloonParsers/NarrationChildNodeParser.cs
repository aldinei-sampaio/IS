namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class NarrationChildNodeParser : BalloonChildNodeParserBase, INarrationChildNodeParser
{
    public NarrationChildNodeParser(
        IElementParser elementParser,
        INarrationTextNodeParser narrationTextNodeParser,
        IChoiceNodeParser choiceNodeParser
    )
    : base(elementParser, narrationTextNodeParser, choiceNodeParser)
    {
    }
}
