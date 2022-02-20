namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class ThoughtNodeParser : DialogNodeParserBase, IThoughtNodeParser
{
    public override string Name => "*";

    public override BalloonType BalloonType => BalloonType.Thought;

    public ThoughtNodeParser(
        IElementParser elementParser,
        IThoughtChildNodeParser thoughtChildNodeParser,
        IMoodNodeParser moodNodeParser,
        ISetNodeParser setNodeParser
    )
        : base(elementParser, thoughtChildNodeParser, moodNodeParser, setNodeParser)
    {
    }
}
