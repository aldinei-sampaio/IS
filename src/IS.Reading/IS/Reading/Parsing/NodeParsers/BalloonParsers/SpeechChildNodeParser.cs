using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class SpeechChildNodeParser : BalloonTextChildNodeParserBase, ISpeechChildNodeParser
{
    public SpeechChildNodeParser(
        IElementParser elementParser, 
        IBalloonTextParser balloonTextParser, 
        IChoiceNodeParser choiceNodeParser
    ) 
    : base(elementParser, balloonTextParser, choiceNodeParser)
    {
    }

    public override string Name => "speech";

    public override BalloonType BalloonType => BalloonType.Speech;
}
