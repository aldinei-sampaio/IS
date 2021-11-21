namespace IS.Reading.EventObjects;

public interface IOpenCloseEvents : ISimpleEvents
{
    public event AsyncEventHandler<EventArgs>? OnOpenAsync;
    public event AsyncEventHandler<EventArgs>? OnCloseAsync;
}
