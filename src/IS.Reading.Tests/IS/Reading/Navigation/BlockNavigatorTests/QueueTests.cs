using IS.Reading.State;

namespace IS.Reading.Navigation.BlockNavigatorTests;

public class QueueTests
{
    private readonly IBlock block;
    private readonly IBlockState blockState;
    private readonly IBlockIterationState blockIterationState;
    private readonly List<INode> nodes = new();
    private readonly INavigationContext context;
    private readonly BlockNavigator sut;

    public QueueTests()
    {
        sut = new();
        block = A.Dummy<IBlock>();
        A.CallTo(() => block.Nodes).Returns(nodes);
        context = A.Dummy<INavigationContext>();
        context.CurrentNode = null;
        
        blockIterationState = A.Dummy<IBlockIterationState>();
        blockIterationState.CurrentNode = null;
        blockIterationState.CurrentNodeIndex = null;
        blockState = A.Dummy<IBlockState>();
        A.CallTo(() => blockState.GetCurrentIteration()).Returns(blockIterationState);
    }

    [Fact]
    public async Task Empty()
    {
        await TestNextAsync(null);
        await TestPreviousAsync(null);
    }

    [Fact]
    public async Task SingleNode()
    {
        var node = FakeNode(context, "N1");
        nodes.Add(node);

        blockIterationState.BackwardStack.Count.Should().Be(0);

        await TestNextAsync(node);
        blockIterationState.BackwardStack.Count.Should().Be(1);

        await TestNextAsync(null);
        blockIterationState.BackwardStack.Count.Should().Be(1);

        await TestPreviousAsync(node);
        blockIterationState.BackwardStack.Count.Should().Be(0);

        await TestPreviousAsync(null);
        blockIterationState.BackwardStack.Count.Should().Be(0);
    }

    [Fact]
    public async Task TwoNodes()
    {
        var node1 = FakeNode(context, "N1");
        nodes.Add(node1);
        var node2 = FakeNode(context, "N2");
        nodes.Add(node2);

        blockIterationState.BackwardStack.Count.Should().Be(0);

        await TestNextAsync(node1);
        blockIterationState.BackwardStack.Count.Should().Be(1);

        await TestNextAsync(node2);
        blockIterationState.BackwardStack.Count.Should().Be(2);

        await TestNextAsync(null);
        blockIterationState.BackwardStack.Count.Should().Be(2);

        await TestPreviousAsync(node2);
        blockIterationState.BackwardStack.Count.Should().Be(1);

        await TestPreviousAsync(node1);
        blockIterationState.BackwardStack.Count.Should().Be(0);

        await TestPreviousAsync(null);
        blockIterationState.BackwardStack.Count.Should().Be(0);
    }

    private async Task TestPreviousAsync(INode node)
    {
        await sut.MoveAsync(block, blockState, context, false);
        blockIterationState.CurrentNode.Should().BeSameAs(node);
    }

    private async Task TestNextAsync(INode node)
    {
        await sut.MoveAsync(block, blockState, context, true);
        blockIterationState.CurrentNode.Should().BeSameAs(node);
    }

    private static INode FakeNode(INavigationContext context, string name)
    {
        var node = A.Fake<INode>(i => i.Strict());
        A.CallTo(() => node.EnterAsync(context)).Returns(Task.FromResult<object>(null));
        A.CallTo(() => node.EnterAsync(context, null)).DoesNothing();
        A.CallTo(() => node.LeaveAsync(context)).DoesNothing();
        A.CallTo(() => node.ToString()).Returns(name);
        A.CallTo(() => node.ChildBlock).Returns(null);
        return node;
    }
}
