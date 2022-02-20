using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class NarrationChildNodeParser : DialogChildNodeParserBase, INarrationChildNodeParser
{
    public NarrationChildNodeParser(
        IElementParser elementParser,
        ITextSourceParser textSourceParser,
        IChoiceNodeParser choiceNodeParser)
        : base(elementParser, textSourceParser, choiceNodeParser)
    {
    }

    override public string Name => ">";
}