namespace IS.Reading.Events;

public class PromptEventObject<T> : IPromptEvents<T>, IPromptEventCaller<T>
{
    public event AsyncEventHandler<EventArgs<T>>? OnOpenAsync;

    Task IPromptEventCaller<T>.OpenAsync(T prompt)
        => OnOpenAsync.InvokeAllAsync(this, new(prompt));
}
