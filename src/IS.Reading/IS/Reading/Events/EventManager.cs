namespace IS.Reading.Events;

public class EventManager : IEventSubscriber, IEventInvoker
{
    private readonly Dictionary<Type, List<object>> subscriptions = new();

    public async Task InvokeAsync<T>(T @event) where T : IReadingEvent
    {
        if (!subscriptions.TryGetValue(typeof(T), out var list))
            return;

        await Task.WhenAll(list.OfType<Func<T, Task>>().Select(i => i.Invoke(@event)));
    }

    public void Subscribe<T>(Func<T, Task> handler) where T : IReadingEvent
    {
        lock(subscriptions)
        {
            if (!subscriptions.TryGetValue(typeof(T), out var list))
            {
                list = new List<object>();
                subscriptions.Add(typeof(T), list);
            }

            list.Add(handler);
        }
    }
}
