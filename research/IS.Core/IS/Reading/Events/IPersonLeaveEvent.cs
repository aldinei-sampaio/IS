namespace IS.Reading.Events;

public interface IPersonLeaveEvent : IReadingEvent
{
    bool IsMainCharacter { get; }
    string PersonName { get; }
}
