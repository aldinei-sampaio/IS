namespace IS.Reading.Navigation.BlockNavigatorTests;

public class BackAndForthTester
{
    public IBlock Block { get; }
    public IContext Context { get; }
    public BlockNavigator Navigator { get; }

    private List<string> log = new();

    public BackAndForthTester()
    {
        Navigator = new BlockNavigator();
        Block = A.Dummy<IBlock>();
        Context = A.Dummy<IContext>();
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
        Block.Current.Should().BeSameAs(item);
    }

    public void AddNode(string normalName, string reversedName)
    {
        var node = new TestNode(normalName, reversedName, null, null);
        Block.ForwardQueue.Enqueue(node);
    }

    public void AddNode(string normalName, string reversedName, Func<bool> condition)
    {
        var conditionObject = A.Fake<ICondition>();
        A.CallTo(() => conditionObject.Evaluate(Context.Variables)).ReturnsLazily(() => condition.Invoke());

        var node = new TestNode(normalName, reversedName, conditionObject, null);
        Block.ForwardQueue.Enqueue(node);
    }

    public void AddLoggedNode(string normalName, string reversedName, Func<bool> condition = null)
    {
        ICondition conditionObject = null;
        if (condition is not null)
        {
            conditionObject = A.Fake<ICondition>();
            A.CallTo(() => conditionObject.Evaluate(Context.Variables)).ReturnsLazily(() => condition.Invoke());
        }

        var node = new TestNode(normalName, reversedName, conditionObject, log);
        Block.ForwardQueue.Enqueue(node);
    }
}

