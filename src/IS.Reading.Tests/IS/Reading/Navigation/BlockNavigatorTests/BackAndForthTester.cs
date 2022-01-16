using IS.Reading.Conditions;
using IS.Reading.State;

namespace IS.Reading.Navigation.BlockNavigatorTests;

public class BackAndForthTester
{
    public IBlock Block { get; }
    public IBlockState BlockState { get; }
    public INavigationContext Context { get; }
    public BlockNavigator Navigator { get; }
    
    private readonly List<string> log = new();
    private readonly List<INode> nodes = new();

    public BackAndForthTester()
    {
        Navigator = new BlockNavigator();
        Block = A.Dummy<IBlock>();
        A.CallTo(() => Block.Nodes).Returns(nodes);
        Context = A.Dummy<INavigationContext>();
        BlockState = A.Dummy<IBlockState>();
        A.CallTo(() => Context.State.BlockStates[0, 0]).Returns(BlockState);
    }

    public void CheckLog(params string[] expectedLogEntries)
    {
        log.Should().BeEquivalentTo(expectedLogEntries);
        log.Clear();
    }

    public async Task MoveAsync(bool forward, params string[] expectedNames)
    {
        if (expectedNames == null)
            await DoMoveAsync(forward, null);
        else
            foreach (var name in expectedNames)
                await DoMoveAsync(forward, name);
    }

    private async Task DoMoveAsync(bool forward, string expectedName)
    {
        var item = (TestNode)await Navigator.MoveAsync(Block, Context, forward);
        var actualName = item?.Name;
        actualName.Should().Be(expectedName);
        BlockState.CurrentNode.Should().BeSameAs(item);
    }

    public void AddNode(string normalName, string reversedName)
        => nodes.Add(new TestNode(normalName, reversedName, null, null));

    public void AddNode(string normalName, string reversedName, Func<bool> condition)
    {
        var conditionObject = A.Fake<ICondition>();
        A.CallTo(() => conditionObject.Evaluate(Context.Variables)).ReturnsLazily(() => condition.Invoke());

        nodes.Add(new TestNode(normalName, reversedName, conditionObject, null));
    }

    public void AddLoggedNode(string normalName, string reversedName, Func<bool> condition = null)
    {
        ICondition conditionObject = null;
        if (condition is not null)
        {
            conditionObject = A.Fake<ICondition>();
            A.CallTo(() => conditionObject.Evaluate(Context.Variables)).ReturnsLazily(() => condition.Invoke());
        }

        nodes.Add(new TestNode(normalName, reversedName, conditionObject, log));
    }
}

