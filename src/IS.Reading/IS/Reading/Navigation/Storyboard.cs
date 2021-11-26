﻿using IS.Reading.Events;

namespace IS.Reading.Navigation;

public class Storyboard : IStoryboard
{
    private readonly ISceneNavigator sceneNavigator;
    private readonly IEventManager eventManager;

    public Storyboard(IBlock rootBlock, ISceneNavigator sceneNavigator, IEventManager eventManager)
    {
        NavigationContext = new NavigationContext(rootBlock, eventManager);
        this.eventManager = eventManager;
        this.sceneNavigator = sceneNavigator;
    }

    public INavigationContext NavigationContext { get; }

    public IEventSubscriber Events => eventManager;

    public Task<bool> MoveAsync(bool forward)
        => sceneNavigator.MoveAsync(NavigationContext, forward);

    public void Dispose()
        => eventManager.Dispose();
}
