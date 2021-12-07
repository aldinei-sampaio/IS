using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class MoodTextNodeParser : GenericTextNodeParserBase, IMoodTextNodeParser
{
    public MoodTextNodeParser(IElementParser elementParser, IMoodTextParser moodTextParser) 
        : base(elementParser, moodTextParser)
    {
    }

    public override string Name => "mood";
}
