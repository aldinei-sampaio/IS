using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class NarrationTextNodeParser : GenericTextNodeParserBase, INarrationTextNodeParser
{
    public NarrationTextNodeParser(IElementParser elementParser, IBalloonTextParser balloonTextParser) 
        : base(elementParser, balloonTextParser)
    {
    }

    public override string Name => "narration";

    public BalloonType BalloonType => BalloonType.Narration;
}
