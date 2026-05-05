using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceIfNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IConditionParser conditionParser;
    private readonly IChoiceDefaultNodeParser defaultNodeParser;
    private readonly IChoiceRandomOrderNodeParser randomOrderNodeParser;
    private readonly IChoiceTimeLimitNodeParser timeLimitNodeParser;
    private readonly IChoiceOptionNodeParser optionNodeParser;
    private readonly ChoiceIfNodeParser sut;

    public ChoiceIfNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        conditionParser = A.Fake<IConditionParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        defaultNodeParser = Helper.FakeParser<IChoiceDefaultNodeParser>("default");
        randomOrderNodeParser = Helper.FakeParser<IChoiceRandomOrderNodeParser>("randomorder");
        timeLimitNodeParser = Helper.FakeParser<IChoiceTimeLimitNodeParser>("timelimit");
        optionNodeParser = Helper.FakeParser<IChoiceOptionNodeParser>("a");

        sut = new(
            elementParser, 
            conditionParser, 
            defaultNodeParser, 
            randomOrderNodeParser, 
            timeLimitNodeParser, 
            optionNodeParser            
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
            defaultNodeParser,
            randomOrderNodeParser,
            timeLimitNodeParser,
            optionNodeParser,
            sut
        );
        sut.ElseBlockSettings.Should().BeOfType<ElementParserSettings.Block>();
        sut.ElseBlockSettings.ChildParsers.Should().BeEquivalentTo(
            defaultNodeParser,
            randomOrderNodeParser,
            timeLimitNodeParser,
            optionNodeParser,
            sut
        );
    }
}
