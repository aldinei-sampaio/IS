using IS.Reading.Conditions;
using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class IfNodeTests
{
    private readonly IVariableDictionary variableDictionary;
    private readonly INavigationContext navigationContext;

    private readonly List<IDecisionBlock> decisionBlocks;
    private readonly IBlock elseBlock;
    private readonly IfNode sut;

    public IfNodeTests()
    {
        variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        A.CallTo(() => navigationContext.Variables).Returns(variableDictionary);

        decisionBlocks = new List<IDecisionBlock>();
        elseBlock = A.Fake<IBlock>();
        sut = new(decisionBlocks, elseBlock);
    }

    [Fact]
    public void Initialization()
    {
        sut.DecisionBlocks.Should().BeSameAs(decisionBlocks);
        sut.ElseBlock.Should().BeSameAs(elseBlock);
        sut.ChildBlock.Should().BeNull();
    }

    private ICondition CreateCondition(bool result)
    {
        var condition = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => condition.Evaluate(variableDictionary)).Returns(result);
        return condition;
    }

    private IDecisionBlock CreateDecisionBlock(ICondition condition, IBlock block)
    {
        var decisionBlock = A.Fake<IDecisionBlock>();
        A.CallTo(() => decisionBlock.Condition).Returns(condition);
        A.CallTo(() => decisionBlock.Block).Returns(block);
        decisionBlocks.Add(decisionBlock);
        return decisionBlock;
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IfAndElse(bool conditionResult)
    {
        var condition = CreateCondition(conditionResult);
        var ifBlock = A.Dummy<IBlock>();
        CreateDecisionBlock(condition, ifBlock);

        sut.ChildBlock.Should().BeNull();
        await sut.EnterAsync(navigationContext);

        sut.ChildBlock.Should().BeSameAs(conditionResult ? ifBlock : elseBlock);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task IfAndElseIfAndElse(int? conditionValue)
    {
        var ifBlocks = new[] { A.Dummy<IBlock>(), A.Dummy<IBlock>(), A.Dummy<IBlock>() };

        CreateDecisionBlock(CreateCondition(conditionValue == 0), ifBlocks[0]);
        CreateDecisionBlock(CreateCondition(conditionValue == 1), ifBlocks[1]);
        CreateDecisionBlock(CreateCondition(conditionValue == 2), ifBlocks[2]);

        sut.ChildBlock.Should().BeNull();
        await sut.EnterAsync(navigationContext);

        sut.ChildBlock.Should().BeSameAs(conditionValue.HasValue ? ifBlocks[conditionValue.Value] : elseBlock);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task EnterAsyncWithState(int? stateValue)
    {
        var ifBlocks = new[] { A.Dummy<IBlock>(), A.Dummy<IBlock>(), A.Dummy<IBlock>() };

        CreateDecisionBlock(CreateCondition(stateValue == 0), ifBlocks[0]);
        CreateDecisionBlock(CreateCondition(stateValue == 1), ifBlocks[1]);
        CreateDecisionBlock(CreateCondition(stateValue == 2), ifBlocks[2]);

        sut.ChildBlock.Should().BeNull();
        await sut.EnterAsync(navigationContext, stateValue);

        sut.ChildBlock.Should().BeSameAs(stateValue.HasValue ? ifBlocks[stateValue.Value] : elseBlock);
    }
}