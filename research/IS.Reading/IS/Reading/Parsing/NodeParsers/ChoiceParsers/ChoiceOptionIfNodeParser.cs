using IS.Reading.Choices;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionIfNodeParser : BuilderIfNodeParserBase<IChoiceOptionPrototype>, IChoiceOptionIfNodeParser
{
    public ChoiceOptionIfNodeParser(
        IElementParser elementParser,
        IConditionParser conditionParser,
        IChoiceOptionTextNodeParser choiceOptionTextNodeParser,
        IChoiceOptionDisabledNodeParser choiceOptionDisabledNodeParser,
        IChoiceOptionIconNodeParser choiceOptionIconNodeParser,
        IChoiceOptionTipNodeParser choiceOptionTipNodeParser
    )
    {
        ElementParser = elementParser;
        ConditionParser = conditionParser;

        IfBlockSettings = new ElementParserSettings.IfBlock(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionTipNodeParser,
            this
        );

        ElseBlockSettings = new ElementParserSettings.Block(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionTipNodeParser,
            this
        );
    }
}
