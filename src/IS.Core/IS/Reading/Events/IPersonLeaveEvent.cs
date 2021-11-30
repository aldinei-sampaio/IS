namespace IS.Reading.Events;

public interface IPersonLeaveEvent : IReadingEvent
{
    bool IsProtagonist { get; }
    string PersonName { get; }
}
