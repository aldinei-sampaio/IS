using IS.Reading.State;

namespace IS.Reading.Navigation.SceneNavigatorTests;

public class HierarchyTester
{
    private readonly INavigationContext navigationContext = A.Dummy<INavigationContext>();
    private readonly IBlockNavigator blockNavigator = A.Fake<IBlockNavigator>(i => i.Strict());
    private readonly SceneNavigator sut;

    public IBlock RootBlock { get; }
    public IBlockState RootBlockState { get; } = new FakeBlockState();

    public HierarchyTester()
    {
        RootBlock = CreateDummyBlock();
        navigationContext.CurrentBlock = null;
        navigationContext.CurrentBlockState = null;
        A.CallTo(() => navigationContext.RootBlock).Returns(RootBlock);
        A.CallTo(() => navigationContext.RootBlockState).Returns(RootBlockState);
        sut = new(blockNavigator);
    }

    private int blockId;
    public IBlock CreateDummyBlock()
    {
        var block = A.Dummy<IBlock>();
        A.CallTo(() => block.ToString()).Returns($"FakeBlock {blockId}");
        A.CallTo(() => block.Id).Returns(blockId++);
        return block;
    }

    public void ConfigureMove(bool forward, params INode[] nodes)
        => ConfigureMove(forward, RootBlock, nodes);

    private void ConfigureMove(bool forward, IBlock block, params INode[] nodes)
    {
        if (nodes is null)
            nodes = new INode[] { null };

        A.CallTo(() => blockNavigator.MoveAsync(block, A<IBlockState>.Ignored, navigationContext, forward))
            .ReturnsNextFromSequence(nodes);
    }

    public async Task MoveAsync(bool forward, INode node)
    {
        (await sut.MoveAsync(navigationContext, forward)).Should().Be(node is not null);
        navigationContext.CurrentNode.Should().BeSameAs(node);
    }

    public static INode CreateNode(string name, IBlock childBlock = null)
        => CreateNode<INode>(name, childBlock);

    public static IPauseNode CreatePauseNode(string name)
        => CreateNode<IPauseNode>(name, null);

    private static T CreateNode<T>(string name, IBlock childBlock = null) where T : class, INode
    {
        var node = A.Fake<T>(i => i.Strict());
        A.CallTo(() => node.ChildBlock).Returns(childBlock);
        A.CallTo(() => node.ToString()).Returns(name);
        return node;
    }

    public ConfigureBuilder MoveWithArgs(bool forward, IBlock block)
        => new(this, forward, block);

    public class ConfigureBuilder
    {
        private readonly HierarchyTester tester;
        private readonly bool forward;
        private readonly IBlock block;

        public ConfigureBuilder(HierarchyTester tester, bool forward, IBlock block)
        {
            this.tester = tester;
            this.forward = forward;
            this.block = block;
        }

        public void MustReturn(params INode[] nodes)
            => tester.ConfigureMove(forward, block, nodes);
    }
}
