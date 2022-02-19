using IS.Reading.Choices;
using IS.Reading.Conditions;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceParentParsingContextBaseTests
{
    private class TestClass : ChoiceParentParsingContextBase<int>
    {
    }

    [Fact]
    public void Initialization()
    {
        var sut = new TestClass();
        sut.Builders.Should().BeEmpty();
    }

    [Fact]
    public void AddDecision()
    {
        var condition = A.Dummy<ICondition>();
        var ifBlock = A.Dummy<IEnumerable<IBuilder<int>>>();
        var elseBlock = A.Dummy<IEnumerable<IBuilder<int>>>();

        var sut = new TestClass();
        sut.AddDecision(condition, ifBlock, elseBlock);
        sut.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<BuilderDecision<int>>()
            .Which.ShouldSatisfy(i =>
            {
                i.Condition.Should().BeSameAs(condition);
                i.IfBlock.Should().BeSameAs(ifBlock);
                i.ElseBlock.Should().BeSameAs(elseBlock);
            });
    }
}
