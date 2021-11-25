using IS.Reading.Events;

namespace IS.Reading.Nodes;

public class TestInvoker : IEventInvoker
{
    public List<IReadingEvent> Received { get; } = new();

    public Task InvokeAsync<T>(T @event) where T : IReadingEvent
    {
        Received.Add(@event);
        return Task.CompletedTask;
    }
}
