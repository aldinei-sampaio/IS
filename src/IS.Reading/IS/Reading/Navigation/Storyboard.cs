using IS.Reading.Events;

namespace IS.Reading.Navigation;

public class Storyboard : IStoryboard
{
    public ISceneNavigator SceneNavigator { get; }

    public IEventManager EventManager { get; }

    public Storyboard(INavigationContext navigationContext, ISceneNavigator sceneNavigator, IEventManager eventManager)
    {
        NavigationContext = navigationContext;
        EventManager = eventManager;
        SceneNavigator = sceneNavigator;
    }

    public INavigationContext NavigationContext { get; }

    public IEventSubscriber Events => EventManager;

    public Task<bool> MoveAsync(bool forward)
        => SceneNavigator.MoveAsync(NavigationContext, forward);

    public void Dispose()
        => EventManager.Dispose();
}
