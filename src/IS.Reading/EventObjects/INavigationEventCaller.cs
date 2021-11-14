namespace IS.Reading.EventObjects;

public interface INavigationEventCaller
{
    Task MoveNextAsync();
    Task MovePreviousAsync();
}
