namespace IS.Reading.Parsing.NodeParsers;

public class TutorialNodeParser : BalloonNodeParserBase, ITutorialNodeParser
{
    public override string Name => "tutorial";

    public override BalloonType BalloonType => BalloonType.Tutorial;

    public TutorialNodeParser(
        IElementParser elementParser,
        IBalloonTextNodeParser balloonTextNodeParser,
        ISetNodeParser setNodeParser
    ) : base(elementParser, balloonTextNodeParser, setNodeParser)
    {
    }
}