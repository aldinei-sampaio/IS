namespace IS.Reading.Events;

public interface INavigationEventCaller
{
    Task MoveNextAsync();
    Task MovePreviousAsync();
}
