using IS.Reading.Conditions;
using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class IfNodeTests
{
    [Fact]
    public void Initialization()
    {
        var condition = A.Dummy<ICondition>();
        var ifBlock = A.Dummy<IBlock>();
        var elseBlock = A.Dummy<IBlock>();

        var sut = new IfNode(condition, ifBlock, elseBlock);
        sut.Condition.Should().BeSameAs(condition);
        sut.IfBlock.Should().BeSameAs(ifBlock);
        sut.ElseBlock.Should().BeSameAs(elseBlock);
        sut.ChildBlock.Should().BeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task EnterAsyncWithoutState(bool conditionResult)
    {
        var variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        var navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        A.CallTo(() => navigationContext.Variables).Returns(variableDictionary);

        var condition = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => condition.Evaluate(variableDictionary)).Returns(conditionResult);

        var ifBlock = A.Dummy<IBlock>();
        var elseBlock = A.Dummy<IBlock>();

        var sut = new IfNode(condition, ifBlock, elseBlock);

        sut.ChildBlock.Should().BeNull();
        await sut.EnterAsync(navigationContext);

        sut.ChildBlock.Should().BeSameAs(conditionResult ? ifBlock : elseBlock);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task EnterAsyncWithState(bool? stateValue)
    {
        var variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        var navigationContext = A.Fake<INavigationContext>(i => i.Strict());

        var condition = A.Fake<ICondition>(i => i.Strict());
        var ifBlock = A.Dummy<IBlock>();
        var elseBlock = A.Dummy<IBlock>();

        var sut = new IfNode(condition, ifBlock, elseBlock);

        sut.ChildBlock.Should().BeNull();
        await sut.EnterAsync(navigationContext, stateValue);

        sut.ChildBlock.Should().BeSameAs((stateValue ?? true) ? ifBlock : elseBlock);
    }
}