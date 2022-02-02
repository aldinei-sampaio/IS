using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class SpeechTextNodeParser : GenericTextNodeParserBase, ISpeechTextNodeParser
{
    public SpeechTextNodeParser(IElementParser elementParser, IBalloonTextParser balloonTextParser) 
        : base(elementParser, balloonTextParser)
    {
    }

    public override string Name => "speech";

    public BalloonType BalloonType => BalloonType.Speech;
}
