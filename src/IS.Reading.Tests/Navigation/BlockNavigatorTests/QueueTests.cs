namespace IS.Reading.Navigation.BlockNavigatorTests;

public class QueueTests
{
    private readonly INavigationBlock block;
    private readonly INavigationContext context;
    private readonly BlockNavigator sut;

    public QueueTests()
    {
        sut = new();
        block = A.Dummy<INavigationBlock>();
        context = A.Dummy<INavigationContext>();
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

    private async Task TestPreviousAsync(INavigationNode node)
        => (await sut.MovePreviousAsync(block, context)).Should().BeSameAs(node);

    private async Task TestNextAsync(INavigationNode node)
        => (await sut.MoveNextAsync(block, context)).Should().BeSameAs(node);

    private static INavigationNode FakeNode(INavigationContext context)
    {
        var node = A.Fake<INavigationNode>();
        A.CallTo(() => node.Condition).Returns(null);
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
