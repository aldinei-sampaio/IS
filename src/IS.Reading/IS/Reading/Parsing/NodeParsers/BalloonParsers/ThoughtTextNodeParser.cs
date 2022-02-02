using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class ThoughtTextNodeParser : GenericTextNodeParserBase, IThoughtTextNodeParser
{
    public ThoughtTextNodeParser(IElementParser elementParser, IBalloonTextParser balloonTextParser) 
        : base(elementParser, balloonTextParser)
    {
    }

    public override string Name => "thought";

    public BalloonType BalloonType => BalloonType.Thought;
}
