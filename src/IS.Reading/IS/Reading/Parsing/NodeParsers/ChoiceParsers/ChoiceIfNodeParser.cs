using IS.Reading.Choices;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceIfNodeParser : BuilderIfNodeParserBase<IChoicePrototype>, IChoiceIfNodeParser
{
    public ChoiceIfNodeParser(
        IElementParser elementParser,
        IConditionParser conditionParser,
        IChoiceDefaultNodeParser defaultNodeParser,
        IChoiceRandomOrderNodeParser randomOrderNodeParser,
        IChoiceTimeLimitNodeParser timeLimitNodeParser,
        IChoiceOptionNodeParser optionNodeParser
    )
    {
        ElementParser = elementParser;
        ConditionParser = conditionParser;

        var parsers = new INodeParser[]
        {
            defaultNodeParser,
            randomOrderNodeParser,
            timeLimitNodeParser,
            optionNodeParser,
            this
        };

        IfBlockSettings = new ElementParserSettings.IfBlock(parsers);
        ElseBlockSettings = new ElementParserSettings.Block(parsers);
    }
}
