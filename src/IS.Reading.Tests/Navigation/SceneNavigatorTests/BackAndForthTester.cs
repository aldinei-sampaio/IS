namespace IS.Reading.Navigation.SceneNavigatorTests;

public class BackAndForthTester
{
    public interface ITestPauseNode : INavigationPauseNode
    {
        public string Name { get; set; }
    }

    private readonly INavigationContext navigationContext = A.Fake<INavigationContext>();
    private readonly INavigationStoryboard navigationStoryboard = A.Dummy<INavigationStoryboard>();
    private readonly SceneNavigator sut;

    private readonly Queue<INavigationNode> queue = new();

    public BackAndForthTester(bool? forward = null)
    {
        var callback = () =>
        {
            queue.TryDequeue(out var item);
            return Task.FromResult(item);
        };

        var blockNavigator = A.Fake<IBlockNavigator>(i => i.Strict());
        if (!forward.HasValue || forward.Value)
        {
            A.CallTo(() => blockNavigator.MoveNextAsync(null, null))
                .WithAnyArguments()
                .ReturnsLazily(callback);
        }
        if (!forward.HasValue || !forward.Value)
        {
            A.CallTo(() => blockNavigator.MovePreviousAsync(null, null))
                .WithAnyArguments()
                .ReturnsLazily(callback);
        }
        sut = new SceneNavigator(blockNavigator);
    }

    public void AddDummy()
    {
        var node = A.Fake<INavigationNode>(i => i.Strict());
        A.CallTo(() => node.ChildBlock).Returns(null);
        queue.Enqueue(node);
    }
    
    public void AddPause(string sceneName)
    {
        var node = A.Fake<ITestPauseNode>();
        A.CallTo(() => node.ChildBlock).Returns(null);
        node.Name = sceneName;
        queue.Enqueue(node);
    }

    public async Task MoveAsync(bool forward, params string[] sceneNames)
    {
        if (sceneNames is null)
        {
            (await sut.MoveAsync(navigationStoryboard, navigationContext, forward)).Should().BeFalse();
            return;
        }
        foreach (var sceneName in sceneNames)
        {
            if (sceneName is null)
            {
                (await sut.MoveAsync(navigationStoryboard, navigationContext, forward)).Should().BeFalse();
            }
            else
            {
                (await sut.MoveAsync(navigationStoryboard, navigationContext, forward)).Should().BeTrue();
                ((ITestPauseNode)navigationStoryboard.CurrentNode).Name.Should().Be(sceneName);
            }
        }
    }
}

