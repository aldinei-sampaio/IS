using IS.Reading.Conditions;
using IS.Reading.State;

namespace IS.Reading.Navigation.BlockNavigatorTests;

public class NavigatorTester
{
    public IBlock Block { get; }
    public IBlockState BlockState { get; }
    public INavigationContext Context { get; }
    public BlockNavigator Navigator { get; }
    
    private readonly List<string> log = new();
    private readonly List<INode> nodes = new();

    public int? CurrentNodeIndex => BlockState.GetCurrentIteration().CurrentNodeIndex;

    public NavigatorTester()
    {
        Navigator = new BlockNavigator();
        Block = A.Dummy<IBlock>();
        A.CallTo(() => Block.Nodes).Returns(nodes);
        Context = A.Dummy<INavigationContext>();
        BlockState = new FakeBlockState();
        A.CallTo(() => Context.RootBlockState).Returns(BlockState);
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
        var item = (FakeNode)await Navigator.MoveAsync(Block, BlockState, Context, forward);
        item?.LastEnteredName.Should().Be(expectedName);
        BlockState.GetCurrentIteration().CurrentNode.Should().BeSameAs(item);
    }

    public FakeNode AddNode(string normalName, string reversedName)
    {
        var node = new FakeNode(normalName, reversedName, null);
        nodes.Add(node);
        return node;
    }

    public void AddLoggedNode(string normalName, string reversedName)
    {
        nodes.Add(new FakeNode(normalName, reversedName, log));
    }
}

