namespace IS.Reading.Navigation.SceneNavigatorTests;

public class HierarchyTests
{
    [Fact]
    public async Task Empty()
    {
        var navigationContext = A.Dummy<INavigationContext>();
        var navigationStoryboard = A.Dummy<IStoryboard>();

        var blockNavigator = A.Fake<IBlockNavigator>(i => i.Strict());
        A.CallTo(() => blockNavigator.MoveAsync(null, null, true)).WithAnyArguments().Returns((INode)null);

        var sut = new SceneNavigator(blockNavigator);

        (await sut.MoveAsync(navigationStoryboard, navigationContext, true)).Should().BeFalse();
        (await sut.MoveAsync(navigationStoryboard, navigationContext, false)).Should().BeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task EmptyWithTester(bool forward)
    {
        var tester = new HierarchyTester();
        tester.MoveWithArgs(forward, tester.RootBlock).MustReturn(null);
        await tester.MoveAsync(forward, null);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Hierarchy1(bool forward)
    {
        var tester = new HierarchyTester();

        var block1 = tester.RootBlock;
        var block2 = A.Dummy<IBlock>();

        var node1 = HierarchyTester.CreateNode("node1", block2);
        var node2 = HierarchyTester.CreatePauseNode("node2");

        tester.MoveWithArgs(forward, block1).MustReturn(node1, null);
        tester.MoveWithArgs(forward, block2).MustReturn(node2, null);

        await tester.MoveAsync(forward, node2);
        await tester.MoveAsync(forward, null);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Hierarchy2(bool forward)
    {
        var tester = new HierarchyTester();

        var block1 = tester.RootBlock;
        var block2 = A.Dummy<IBlock>();
        var block3 = A.Dummy<IBlock>();

        var blockNode1 = HierarchyTester.CreateNode("blockNode1", block2);
        var blockNode2 = HierarchyTester.CreateNode("blockNode2", block3);
        var pause1 = HierarchyTester.CreatePauseNode("pause1");
        var pause2 = HierarchyTester.CreatePauseNode("pause2");
        var pause3 = HierarchyTester.CreatePauseNode("pause3");

        tester.MoveWithArgs(forward, block1).MustReturn(blockNode1, blockNode2, null);
        tester.MoveWithArgs(forward, block2).MustReturn(pause1, null);
        tester.MoveWithArgs(forward, block3).MustReturn(pause2, pause3, null);

        await tester.MoveAsync(forward, pause1);
        await tester.MoveAsync(forward, pause2);
        await tester.MoveAsync(forward, pause3);
        await tester.MoveAsync(forward, null);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Hierarchy3(bool forward)
    {
        var tester = new HierarchyTester();

        var block1 = tester.RootBlock;
        var block2 = A.Dummy<IBlock>();
        var block3 = A.Dummy<IBlock>();
        var block4 = A.Dummy<IBlock>();

        var blockNode1 = HierarchyTester.CreateNode("blockNode1", block2);
        var blockNode2 = HierarchyTester.CreateNode("blockNode2", block3);
        var blockNode3 = HierarchyTester.CreateNode("blockNode3", block4);
        var pause1 = HierarchyTester.CreatePauseNode("pause1");
        var pause2 = HierarchyTester.CreatePauseNode("pause2");
        var pause3 = HierarchyTester.CreatePauseNode("pause3");
        var pause4 = HierarchyTester.CreatePauseNode("pause4");
        var pause5 = HierarchyTester.CreatePauseNode("pause5");

        tester.MoveWithArgs(forward, block1).MustReturn(blockNode1, blockNode3, null);
        tester.MoveWithArgs(forward, block2).MustReturn(pause1, blockNode2, pause4, null);
        tester.MoveWithArgs(forward, block3).MustReturn(pause2, pause3, null);
        tester.MoveWithArgs(forward, block4).MustReturn(pause5, null);

        await tester.MoveAsync(forward, pause1);
        await tester.MoveAsync(forward, pause2);
        await tester.MoveAsync(forward, pause3);
        await tester.MoveAsync(forward, pause4);
        await tester.MoveAsync(forward, pause5);
        await tester.MoveAsync(forward, null);
    }
}
