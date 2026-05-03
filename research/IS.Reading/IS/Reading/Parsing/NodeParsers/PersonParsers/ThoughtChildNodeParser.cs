using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class ThoughtChildNodeParser : DialogChildNodeParserBase, IThoughtChildNodeParser
{
    public ThoughtChildNodeParser(
        IElementParser elementParser,
        ITextSourceParser textSourceParser,
        IChoiceNodeParser choiceNodeParser)
        : base(elementParser, textSourceParser, choiceNodeParser)
    {
    }

    override public string Name => "*";
}