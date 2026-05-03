namespace IS.Reading.Events;

public interface IMainCharacterChangeEvent : IReadingEvent
{
    string? PersonName { get; }
}
