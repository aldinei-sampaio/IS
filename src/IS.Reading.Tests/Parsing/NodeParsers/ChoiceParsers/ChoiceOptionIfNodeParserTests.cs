using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionIfNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IConditionParser conditionParser;
    private readonly IChoiceOptionTextNodeParser choiceOptionTextNodeParser;
    private readonly IChoiceOptionDisabledNodeParser choiceOptionDisabledNodeParser;
    private readonly IChoiceOptionIconNodeParser choiceOptionIconNodeParser;
    private readonly IChoiceOptionTipNodeParser choiceOptionTipNodeParser;
    private readonly ChoiceOptionIfNodeParser sut;

    public ChoiceOptionIfNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        conditionParser = A.Fake<IConditionParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        choiceOptionTextNodeParser = Helper.FakeParser<IChoiceOptionTextNodeParser>("a");
        choiceOptionDisabledNodeParser = Helper.FakeParser<IChoiceOptionDisabledNodeParser>("disabled");
        choiceOptionIconNodeParser = Helper.FakeParser<IChoiceOptionIconNodeParser>("icon");
        choiceOptionTipNodeParser = Helper.FakeParser<IChoiceOptionTipNodeParser>("tip");

        sut = new(
            elementParser, 
            conditionParser, 
            choiceOptionTextNodeParser, 
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser, 
            choiceOptionTipNodeParser
        );
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("if");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.ConditionParser.Should().BeSameAs(conditionParser);
        sut.IfBlockSettings.Should().BeOfType<ElementParserSettings.IfBlock>();
        sut.IfBlockSettings.ChildParsers.Should().BeEquivalentTo(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionTipNodeParser,
            sut
        );
        sut.ElseBlockSettings.Should().BeOfType<ElementParserSettings.Block>();
        sut.ElseBlockSettings.ChildParsers.Should().BeEquivalentTo(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionTipNodeParser,
            sut
        );
    }
}
