namespace IS.Reading.Parsing.NodeParsers;

public class NarrationNodeParser : DialogNodeParserBase, INarrationNodeParser
{
    public override string Name => ">";

    public override BalloonType BalloonType => BalloonType.Narration;

    public NarrationNodeParser(
        IElementParser elementParser,
        INarrationChildNodeParser narrationChildNodeParser,
        ISetNodeParser setNodeParser
    ) : base(elementParser, narrationChildNodeParser, setNodeParser)
    {
    }
}
