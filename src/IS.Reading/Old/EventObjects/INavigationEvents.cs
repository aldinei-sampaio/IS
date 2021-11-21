namespace IS.Reading.EventObjects;

public interface INavigationEvents
{
    event AsyncEventHandler<EventArgs>? OnMoveNextAsync;
    event AsyncEventHandler<EventArgs>? OnMovePreviousAsync;
}
