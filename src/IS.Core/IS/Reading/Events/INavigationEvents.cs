namespace IS.Reading.Events;

public interface INavigationEvents
{
    event AsyncEventHandler<EventArgs>? OnMoveNextAsync;
    event AsyncEventHandler<EventArgs>? OnMovePreviousAsync;
}
