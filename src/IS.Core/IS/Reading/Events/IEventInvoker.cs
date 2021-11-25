namespace IS.Reading.Events;

public interface IEventInvoker
{
    Task InvokeAsync<T>(T @event) where T : IReadingEvent;
}
