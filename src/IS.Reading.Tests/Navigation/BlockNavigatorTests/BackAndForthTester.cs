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

    public async Task ForwardAsync(params string[] expectedNames)
    {
        if (expectedNames == null)
            await DoNextAsync(null);
        else
            foreach (var name in expectedNames)
                await DoNextAsync(name);
    }

    private async Task DoNextAsync(string expectedName)
    {
        var item = (TestNode)await Navigator.MoveNextAsync(Block, Context);
        var actualName = item?.Name;
        actualName.Should().Be(expectedName);
        Block.Current.Should().BeSameAs(item);
    }

    public async Task BackwardAsync(params string[] expectedNames)
    {
        if (expectedNames == null)
            await DoPreviousAsync(null);
        else
            foreach (var name in expectedNames)
                await DoPreviousAsync(name);
    }

    private async Task DoPreviousAsync(string expectedName)
    {
        var item = (TestNode)await Navigator.MovePreviousAsync(Block, Context);
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

