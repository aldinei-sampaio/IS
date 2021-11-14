namespace IS.Reading.EventObjects;

public class OpenCloseEventObject : SimpleEventObject, IOpenCloseEvents, IOpenCloseEventCaller
{
    public event AsyncEventHandler<EventArgs>? OnOpenAsync;
    public event AsyncEventHandler<EventArgs>? OnCloseAsync;

    Task IOpenCloseEventCaller.OpenAsync()
        => OnOpenAsync.InvokeAllAsync(this, EventArgs.Empty);

    Task IOpenCloseEventCaller.CloseAsync()
        => OnCloseAsync.InvokeAllAsync(this, EventArgs.Empty);
}
