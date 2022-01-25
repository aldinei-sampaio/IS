using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class ThoughtChildNodeParser : BalloonChildNodeParserBase, IThoughtChildNodeParser
{
    public ThoughtChildNodeParser(
        IElementParser elementParser,
        ITextSourceParser textSourceParser,
        IThoughtTextNodeParser thoughtTextNodeParser, 
        IChoiceNodeParser choiceNodeParser
    ) 
    : base(elementParser, textSourceParser, thoughtTextNodeParser, choiceNodeParser)
    {
    }
}
