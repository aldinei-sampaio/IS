namespace IS.Reading.Events;

public interface IEventSubscriber
{
    void Subscribe<T>(Func<T, Task> handler) where T : IReadingEvent;
    void Subscribe(Func<IReadingEvent, Task> handler);
    void Unsubscribe<T>(Func<T, Task> handler) where T : IReadingEvent;
    void Unsubscribe(Func<IReadingEvent, Task> handler);
    int SubscriptionCount { get; }
}
