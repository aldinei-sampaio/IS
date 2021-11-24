namespace IS.Reading.Events;

public interface IPromptEvents<T>
{
    public event AsyncEventHandler<EventArgs<T>>? OnOpenAsync;
}
