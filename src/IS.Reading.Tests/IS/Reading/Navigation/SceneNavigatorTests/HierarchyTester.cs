namespace IS.Reading.Navigation.SceneNavigatorTests;

public class HierarchyTester
{
    private readonly IContext navigationContext = A.Dummy<IContext>();
    private readonly IStoryboard navigationStoryboard = A.Dummy<IStoryboard>();
    private readonly IBlockNavigator blockNavigator = A.Fake<IBlockNavigator>(i => i.Strict());
    private readonly SceneNavigator sut;

    public IBlock RootBlock => navigationStoryboard.CurrentBlock;

    public HierarchyTester()
    {
        sut = new(blockNavigator);
    }

    private void ConfigureMove(bool forward, IBlock block, params INode[] nodes)
    {
        if (nodes is null)
            nodes = new INode[] { null };

        A.CallTo(() => blockNavigator.MoveAsync(block, navigationContext, forward))
            .ReturnsNextFromSequence(nodes);
    }

    public async Task MoveAsync(bool forward, INode node)
    {
        (await sut.MoveAsync(navigationStoryboard, navigationContext, forward)).Should().Be(node is not null);
        navigationStoryboard.CurrentNode.Should().BeSameAs(node);
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
        => new ConfigureBuilder(this, forward, block);

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
