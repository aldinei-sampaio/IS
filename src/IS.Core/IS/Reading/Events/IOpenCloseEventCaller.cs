namespace IS.Reading.Events;

public interface IOpenCloseEventCaller : ISimpleEventCaller
{
    Task OpenAsync();
    Task CloseAsync();
}
