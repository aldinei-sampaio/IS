namespace IS.Reading.Events;

public interface ISimpleEvents
{
    event AsyncEventHandler<EventArgs<string>>? OnChangeAsync;
}
