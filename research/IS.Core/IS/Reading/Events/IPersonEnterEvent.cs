namespace IS.Reading.Events;

public interface IPersonEnterEvent : IReadingEvent
{
    bool IsMainCharacter { get; }
    string PersonName { get; }
}
