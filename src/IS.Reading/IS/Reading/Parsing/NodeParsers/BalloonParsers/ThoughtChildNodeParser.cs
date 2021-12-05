using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class ThoughtChildNodeParser : BalloonTextChildNodeParserBase, IThoughtChildNodeParser
{
    public ThoughtChildNodeParser(
        IElementParser elementParser, 
        IBalloonTextParser balloonTextParser, 
        IChoiceNodeParser choiceNodeParser
    ) 
    : base(elementParser, balloonTextParser, choiceNodeParser)
    {
    }

    public override string Name => "thought";

    public override BalloonType BalloonType => BalloonType.Thought;
}
