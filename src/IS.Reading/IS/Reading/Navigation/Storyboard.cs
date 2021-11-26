using IS.Reading.Events;

namespace IS.Reading.Navigation;

public class Storyboard : IStoryboard
{
    private readonly ISceneNavigator sceneNavigator;
    private readonly EventManager eventManager;

    public Storyboard(IBlock rootBlock, ISceneNavigator sceneNavigator)
    {
        eventManager = new();
        NavigationContext = new NavigationContext(rootBlock, eventManager);
        this.sceneNavigator = sceneNavigator;
    }

    public INavigationContext NavigationContext { get; }

    public IEventSubscriber Events => eventManager;

    public Task<bool> MoveAsync(bool forward)
        => sceneNavigator.MoveAsync(NavigationContext, forward);

    public Task LoadStateAsync(Stream stream)
    {
        throw new NotImplementedException();
    }

    public Task SaveStateAsync(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
        => eventManager.Dispose();
}
