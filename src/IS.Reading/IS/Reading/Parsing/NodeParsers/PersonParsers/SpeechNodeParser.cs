namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class SpeechNodeParser : BalloonNodeParserBase, ISpeechNodeParser
{
    public override string Name => "speech";

    public override BalloonType BalloonType => BalloonType.Speech;

    public SpeechNodeParser(
        IElementParser elementParser, 
        IBalloonTextNodeParser balloonTextNodeParser, 
        IMoodNodeParser moodNodeParser, 
        ISetNodeParser setNodeParser
    ) 
        : base(elementParser, balloonTextNodeParser, moodNodeParser, setNodeParser)
    {
    }
}
