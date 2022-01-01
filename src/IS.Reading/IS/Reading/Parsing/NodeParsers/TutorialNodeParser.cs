using IS.Reading.Parsing.NodeParsers.BalloonParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class TutorialNodeParser : BalloonTextNodeParserBase, ITutorialNodeParser
{
    public TutorialNodeParser(
        IElementParser elementParser, 
        ITutorialChildNodeParser childParser,
        ISetNodeParser setNodeParser,
        IUnsetNodeParser unsetNodeParser
    )
        : base(elementParser, childParser, setNodeParser, unsetNodeParser)
    {        
    }
}
