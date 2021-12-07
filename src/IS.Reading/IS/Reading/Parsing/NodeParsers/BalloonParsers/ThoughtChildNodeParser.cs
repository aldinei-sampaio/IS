namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class ThoughtChildNodeParser : BalloonChildNodeParserBase, IThoughtChildNodeParser
{
    public ThoughtChildNodeParser(
        IElementParser elementParser, 
        IThoughtTextNodeParser thoughtTextNodeParser, 
        IChoiceNodeParser choiceNodeParser
    ) 
    : base(elementParser, thoughtTextNodeParser, choiceNodeParser)
    {
    }
}
