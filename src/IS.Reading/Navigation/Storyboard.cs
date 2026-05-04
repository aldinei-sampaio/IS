using IS.Reading.Events;

namespace IS.Reading.Navigation;

public class Storyboard(INavigationContext navigationContext, ISceneNavigator sceneNavigator, IEventManager eventManager) : IStoryboard
{
    public INavigationContext NavigationContext { get; } = navigationContext;

    public IEventManager EventManager { get; } = eventManager;

    public ISceneNavigator SceneNavigator { get; } = sceneNavigator;

    public IEventSubscriber Events => EventManager;

    public async Task<bool> MoveAsync(bool forward)
    {
        if (IsWaitingForChoice)
        {
            if (forward)
                throw new InvalidOperationException("Valor de escolha é requerido para prosseguir.");

            NavigationContext.State.WaitingFor = null;
        }

        return await SceneNavigator.MoveAsync(NavigationContext, forward);
    }

    public bool IsWaitingForChoice => NavigationContext.State.WaitingFor is not null;

    public void Input(string value)
    {
        if (NavigationContext.State.WaitingFor is null)
            throw new InvalidOperationException("Valor de escolha não é esperado neste momento.");

        NavigationContext.Variables[NavigationContext.State.WaitingFor] = value;
        NavigationContext.State.WaitingFor = null;
    }

    public void Dispose()
        => EventManager.Dispose();
}
