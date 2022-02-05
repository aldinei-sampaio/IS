namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class ThoughtNodeParser : BalloonNodeParserBase, IThoughtNodeParser
{
    public override string Name => "thought";

    public override BalloonType BalloonType => BalloonType.Speech;

    public ThoughtNodeParser(
        IElementParser elementParser,
        IBalloonTextNodeParser balloonTextNodeParser,
        IMoodNodeParser moodNodeParser,
        ISetNodeParser setNodeParser
    )
        : base(elementParser, balloonTextNodeParser, moodNodeParser, setNodeParser)
    {
    }
}
