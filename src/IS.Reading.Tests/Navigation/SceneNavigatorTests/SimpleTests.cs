namespace IS.Reading.Navigation.SceneNavigatorTests;

public class SimpleTests
{
    [Fact]
    public async Task Empty()
    {
        var navigationContext = A.Dummy<INavigationContext>();
        var navigationStoryboard = A.Dummy<INavigationStoryboard>();

        var blockNavigator = A.Fake<IBlockNavigator>(i => i.Strict());
        A.CallTo(() => blockNavigator.MoveNextAsync(null, null)).WithAnyArguments().Returns((INavigationNode)null);
        A.CallTo(() => blockNavigator.MovePreviousAsync(null, null)).WithAnyArguments().Returns((INavigationNode)null);

        var sut = new SceneNavigator(blockNavigator);

        (await sut.MoveAsync(navigationStoryboard, navigationContext, true)).Should().BeFalse();
        (await sut.MoveAsync(navigationStoryboard, navigationContext, false)).Should().BeFalse();
    }

    [Fact]
    public async Task Hierarchy1()
    {
        var navigationContext = A.Dummy<INavigationContext>();
        var navigationStoryboard = A.Dummy<INavigationStoryboard>();

        var block1 = navigationStoryboard.CurrentBlock;
        var block2 = A.Dummy<INavigationBlock>();
        
        var node1 = A.Fake<INavigationNode>(i => i.Strict());
        A.CallTo(() => node1.ChildBlock).Returns(block2);
        
        var node2 = A.Fake<INavigationPauseNode>(i => i.Strict());
        A.CallTo(() => node2.ChildBlock).Returns(null);

        var blockNavigator = A.Fake<IBlockNavigator>(i => i.Strict());
        A.CallTo(() => blockNavigator.MoveNextAsync(block1, navigationContext)).ReturnsNextFromSequence(node1, null);
        A.CallTo(() => blockNavigator.MoveNextAsync(block2, navigationContext)).ReturnsNextFromSequence(node2, null);

        var sut = new SceneNavigator(blockNavigator);

        (await sut.MoveAsync(navigationStoryboard, navigationContext, true)).Should().BeTrue();
        navigationStoryboard.CurrentNode.Should().BeSameAs(node2);
        (await sut.MoveAsync(navigationStoryboard, navigationContext, true)).Should().BeFalse();
        navigationStoryboard.CurrentNode.Should().BeNull();
    }

    [Fact]
    public async Task Hierarchy2()
    {
        var nc = A.Dummy<INavigationContext>();
        var ns = A.Dummy<INavigationStoryboard>();

        var block1 = ns.CurrentBlock;
        var block2 = A.Dummy<INavigationBlock>();
        var block3 = A.Dummy<INavigationBlock>();

        var blockNode1 = CreateNode<INavigationNode>("blockNode1", block2);
        var blockNode2 = CreateNode<INavigationNode>("blockNode2", block3);
        var pause1 = CreateNode<INavigationPauseNode>("pause1", null);
        var pause2 = CreateNode<INavigationPauseNode>("pause2", null);
        var pause3 = CreateNode<INavigationPauseNode>("pause3", null);

        var bn = A.Fake<IBlockNavigator>(i => i.Strict());
        A.CallTo(() => bn.MoveNextAsync(block1, nc)).ReturnsNextFromSequence(blockNode1, blockNode2, null);
        A.CallTo(() => bn.MoveNextAsync(block2, nc)).ReturnsNextFromSequence(pause1, null);
        A.CallTo(() => bn.MoveNextAsync(block3, nc)).ReturnsNextFromSequence(pause2, pause3, null);

        var sut = new SceneNavigator(bn);

        await CheckAsync(pause1);
        await CheckAsync(pause2);
        await CheckAsync(pause3);
        await CheckAsync(null);

        async Task CheckAsync(INavigationNode node)
        {
            (await sut.MoveAsync(ns, nc, true)).Should().Be(node is not null);
            ns.CurrentNode.Should().BeSameAs(node);
        }
    }

    [Fact]
    public async Task Hierarchy3()
    {
        var nc = A.Dummy<INavigationContext>();
        var ns = A.Dummy<INavigationStoryboard>();

        var block1 = ns.CurrentBlock;
        var block2 = A.Dummy<INavigationBlock>();
        var block3 = A.Dummy<INavigationBlock>();
        var block4 = A.Dummy<INavigationBlock>();

        var blockNode1 = CreateNode<INavigationNode>("blockNode1", block2);
        var blockNode2 = CreateNode<INavigationNode>("blockNode2", block3);
        var blockNode3 = CreateNode<INavigationNode>("blockNode3", block4);
        var pause1 = CreateNode<INavigationPauseNode>("pause1", null);
        var pause2 = CreateNode<INavigationPauseNode>("pause2", null);
        var pause3 = CreateNode<INavigationPauseNode>("pause3", null);
        var pause4 = CreateNode<INavigationPauseNode>("pause4", null);
        var pause5 = CreateNode<INavigationPauseNode>("pause5", null);

        var bn = A.Fake<IBlockNavigator>(i => i.Strict());
        A.CallTo(() => bn.MoveNextAsync(block1, nc)).ReturnsNextFromSequence(blockNode1, blockNode3, null);
        A.CallTo(() => bn.MoveNextAsync(block2, nc)).ReturnsNextFromSequence(pause1, blockNode2, pause4, null);
        A.CallTo(() => bn.MoveNextAsync(block3, nc)).ReturnsNextFromSequence(pause2, pause3, null);
        A.CallTo(() => bn.MoveNextAsync(block4, nc)).ReturnsNextFromSequence(pause5, null);

        var sut = new SceneNavigator(bn);

        await CheckAsync(pause1);
        await CheckAsync(pause2);
        await CheckAsync(pause3);
        await CheckAsync(pause4);
        await CheckAsync(pause5);
        await CheckAsync(null);

        async Task CheckAsync(INavigationNode node)
        {
            (await sut.MoveAsync(ns, nc, true)).Should().Be(node is not null);
            ns.CurrentNode.Should().BeSameAs(node);
        }
    }

    [Fact]
    public async Task Hierarchy3_Backwards()
    {
        var nc = A.Dummy<INavigationContext>();
        var ns = A.Dummy<INavigationStoryboard>();

        var block1 = ns.CurrentBlock;
        var block2 = A.Dummy<INavigationBlock>();
        var block3 = A.Dummy<INavigationBlock>();
        var block4 = A.Dummy<INavigationBlock>();

        var blockNode1 = CreateNode<INavigationNode>("blockNode1", block2);
        var blockNode2 = CreateNode<INavigationNode>("blockNode2", block3);
        var blockNode3 = CreateNode<INavigationNode>("blockNode3", block4);
        var pause1 = CreateNode<INavigationPauseNode>("pause1", null);
        var pause2 = CreateNode<INavigationPauseNode>("pause2", null);
        var pause3 = CreateNode<INavigationPauseNode>("pause3", null);
        var pause4 = CreateNode<INavigationPauseNode>("pause4", null);
        var pause5 = CreateNode<INavigationPauseNode>("pause5", null);

        var bn = A.Fake<IBlockNavigator>(i => i.Strict());
        A.CallTo(() => bn.MovePreviousAsync(block1, nc)).ReturnsNextFromSequence(blockNode1, blockNode3, null);
        A.CallTo(() => bn.MovePreviousAsync(block2, nc)).ReturnsNextFromSequence(pause1, blockNode2, pause4, null);
        A.CallTo(() => bn.MovePreviousAsync(block3, nc)).ReturnsNextFromSequence(pause2, pause3, null);
        A.CallTo(() => bn.MovePreviousAsync(block4, nc)).ReturnsNextFromSequence(pause5, null);

        var sut = new SceneNavigator(bn);

        await CheckAsync(pause1);
        await CheckAsync(pause2);
        await CheckAsync(pause3);
        await CheckAsync(pause4);
        await CheckAsync(pause5);
        await CheckAsync(null);

        async Task CheckAsync(INavigationNode node)
        {
            (await sut.MoveAsync(ns, nc, false)).Should().Be(node is not null);
            ns.CurrentNode.Should().BeSameAs(node);
        }
    }

    private static T CreateNode<T>(string name, INavigationBlock childBlock) where T : class, INavigationNode
    {
        var node = A.Fake<T>(i => i.Strict());
        A.CallTo(() => node.ChildBlock).Returns(childBlock);
        A.CallTo(() => node.ToString()).Returns(name);
        return node;
    }
}
