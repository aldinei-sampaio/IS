namespace IS.Reading.Events;

public interface IProtagonistChangeEvent : IReadingEvent
{
    string? Name { get; }
}
