namespace IS.Reading.Events;

public interface IProtagonistChangeEvent : IReadingEvent
{
    string? PersonName { get; }
}
