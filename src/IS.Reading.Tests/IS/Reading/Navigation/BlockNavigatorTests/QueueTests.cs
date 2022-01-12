namespace IS.Reading.Navigation.BlockNavigatorTests;

public class QueueTests
{
    private readonly IBlock block;
    private readonly List<INode> nodes = new();
    private readonly INavigationContext context;
    private readonly BlockNavigator sut;

    public QueueTests()
    {
        sut = new();
        block = A.Dummy<IBlock>();
        A.CallTo(() => block.Nodes).Returns(nodes);
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
        nodes.Add(node);

        block.BackwardStack.Count.Should().Be(0);

        await TestNextAsync(node);
        block.BackwardStack.Count.Should().Be(1);

        await TestNextAsync(null);
        block.BackwardStack.Count.Should().Be(1);

        await TestPreviousAsync(node);
        block.BackwardStack.Count.Should().Be(0);

        await TestPreviousAsync(null);
        block.BackwardStack.Count.Should().Be(0);
    }

    [Fact]
    public async Task TwoNodes()
    {
        var node1 = FakeNode(context);
        nodes.Add(node1);
        var node2 = FakeNode(context);
        nodes.Add(node2);

        block.BackwardStack.Count.Should().Be(0);

        await TestNextAsync(node1);
        block.BackwardStack.Count.Should().Be(1);

        await TestNextAsync(node2);
        block.BackwardStack.Count.Should().Be(2);

        await TestNextAsync(null);
        block.BackwardStack.Count.Should().Be(2);

        await TestPreviousAsync(node2);
        block.BackwardStack.Count.Should().Be(1);

        await TestPreviousAsync(node1);
        block.BackwardStack.Count.Should().Be(0);

        await TestPreviousAsync(null);
        block.BackwardStack.Count.Should().Be(0);
    }

    private async Task TestPreviousAsync(INode node)
        => (await sut.MoveAsync(block, context, false)).Should().BeSameAs(node);

    private async Task TestNextAsync(INode node)
        => (await sut.MoveAsync(block, context, true)).Should().BeSameAs(node);

    private static INode FakeNode(INavigationContext context)
    {
        var node = A.Fake<INode>();
        A.CallTo(() => node.When).Returns(null);
        A.CallTo(() => node.EnterAsync(context)).Returns(node);
        return node;
    }
}
