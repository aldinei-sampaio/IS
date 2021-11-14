namespace IS.Reading.EventObjects;

public class NavigationEventObject : INavigationEvents, INavigationEventCaller
{
    public event AsyncEventHandler<EventArgs>? OnMoveNextAsync;
    public event AsyncEventHandler<EventArgs>? OnMovePreviousAsync;

    public Task MoveNextAsync()
        => OnMoveNextAsync.InvokeAllAsync(this, EventArgs.Empty);

    public Task MovePreviousAsync()
        => OnMovePreviousAsync.InvokeAllAsync(this, EventArgs.Empty);
}
