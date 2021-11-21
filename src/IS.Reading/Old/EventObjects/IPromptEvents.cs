namespace IS.Reading.EventObjects;

public interface IPromptEvents<T>
{
    public event AsyncEventHandler<EventArgs<T>>? OnOpenAsync;
}
