namespace IS.Reading.EventObjects;

public interface IOpenCloseEventCaller : ISimpleEventCaller
{
    Task OpenAsync();
    Task CloseAsync();
}
