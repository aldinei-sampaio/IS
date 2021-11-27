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

    public T Single<T>() where T : IReadingEvent
    {
        Received.Should().HaveCount(1);
        return Received[0].Should().BeAssignableTo<T>().Which;
    }
}
