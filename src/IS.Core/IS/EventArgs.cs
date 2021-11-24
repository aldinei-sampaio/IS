namespace IS.Reading;

public class EventArgs<T> : EventArgs
{
    public T Data { get; }
    public EventArgs(T data)
    {
        Data = data;
    }

    public override string? ToString() => Data?.ToString();
}
