namespace IS.Reading.EventObjects;

public interface ISimpleEvents
{
    event AsyncEventHandler<EventArgs<string>>? OnChangeAsync;
}
