namespace IS.Reading.Navigation.BlockNavigatorTests;

public class QueueTests
{
    private readonly IBlock block;
    private readonly IContext context;
    private readonly BlockNavigator sut;

    public QueueTests()
    {
        sut = new();
        block = A.Dummy<IBlock>();
        context = A.Dummy<IContext>();
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
        var node = FakeNode(context);
        block.ForwardQueue.Enqueue(node);

        CheckQueueCount(1, 0, 0);

        await TestNextAsync(node);
        CheckQueueCount(0, 0, 1);

        await TestNextAsync(null);
        CheckQueueCount(0, 0, 1);

        await TestPreviousAsync(node);
        CheckQueueCount(0, 1, 0);

        await TestPreviousAsync(null);
        CheckQueueCount(0, 1, 0);
    }

    [Fact]
    public async Task TwoNodes()
    {
        var node1 = FakeNode(context);
        block.ForwardQueue.Enqueue(node1);
        var node2 = FakeNode(context);
        block.ForwardQueue.Enqueue(node2);

        CheckQueueCount(2, 0, 0);

        await TestNextAsync(node1);
        CheckQueueCount(1, 0, 1);

        await TestNextAsync(node2);
        CheckQueueCount(0, 0, 2);

        await TestNextAsync(null);
        CheckQueueCount(0, 0, 2);

        await TestPreviousAsync(node2);
        CheckQueueCount(0, 1, 1);

        await TestPreviousAsync(node1);
        CheckQueueCount(0, 2, 0);

        await TestPreviousAsync(null);
        CheckQueueCount(0, 2, 0);
    }

    private async Task TestPreviousAsync(INode node)
        => (await sut.MoveAsync(block, context, false)).Should().BeSameAs(node);

    private async Task TestNextAsync(INode node)
        => (await sut.MoveAsync(block, context, true)).Should().BeSameAs(node);

    private static INode FakeNode(IContext context)
    {
        var node = A.Fake<INode>();
        A.CallTo(() => node.When).Returns(null);
        A.CallTo(() => node.EnterAsync(context)).Returns(node);
        return node;
    }

    private void CheckQueueCount(int forwardQueueCount, int forwardStackCount, int backwardStackCount)
    {
        block.ForwardQueue.Count.Should().Be(forwardQueueCount);
        block.ForwardStack.Count.Should().Be(forwardStackCount);
        block.BackwardStack.Count.Should().Be(backwardStackCount);
    }
}
