namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class SpeechNodeParser : DialogNodeParserBase, ISpeechNodeParser
{
    public override string Name => "-";

    public override BalloonType BalloonType => BalloonType.Speech;

    public SpeechNodeParser(
        IElementParser elementParser, 
        ISpeechChildNodeParser speechChildNodeParser, 
        IMoodNodeParser moodNodeParser, 
        ISetNodeParser setNodeParser
    ) 
        : base(elementParser, speechChildNodeParser, moodNodeParser, setNodeParser)
    {
    }
}
