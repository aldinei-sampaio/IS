namespace IS.Reading.Events;

public class EventManager : IEventManager
{
    private readonly Dictionary<Type, List<object>> individualSubscriptions = new();
    private readonly List<Func<IReadingEvent, Task>> allSubscriptions = new();

    public int SubscriptionCount => individualSubscriptions.Count + allSubscriptions.Count;

    public void Dispose()
    {
        individualSubscriptions.Clear();
        allSubscriptions.Clear();
    }

    public async Task InvokeAsync<T>(T @event) where T : IReadingEvent
    {
        if (allSubscriptions.Count > 0)
            await Task.WhenAll(allSubscriptions.Select(i => i.Invoke(@event)));

        if (!individualSubscriptions.TryGetValue(typeof(T), out var list))
            return;

        await Task.WhenAll(list.OfType<Func<T, Task>>().Select(i => i.Invoke(@event)));
    }

    public void Subscribe<T>(Func<T, Task> handler) where T : IReadingEvent
    {
        lock(individualSubscriptions)
        {
            if (!individualSubscriptions.TryGetValue(typeof(T), out var list))
            {
                list = new List<object>();
                individualSubscriptions.Add(typeof(T), list);
            }

            list.Add(handler);
        }
    }

    public void Subscribe(Func<IReadingEvent, Task> handler)
        => allSubscriptions.Add(handler);
}
