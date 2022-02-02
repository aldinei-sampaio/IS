namespace IS.Reading.Parsing.NodeParsers;

public class NarrationNodeParser : BalloonNodeParserBase, INarrationNodeParser
{
    public override string Name => "narration";

    public override BalloonType BalloonType => BalloonType.Narration;

    public NarrationNodeParser(
        IElementParser elementParser,
        IBalloonTextNodeParser balloonTextNodeParser,
        ISetNodeParser setNodeParser
    ) : base(elementParser, balloonTextNodeParser, setNodeParser)
    {
    }
}