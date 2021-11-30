namespace IS.Reading.Events;

public interface IPersonEnterEvent : IReadingEvent
{
    bool IsProtagonist { get; }
    string PersonName { get; }
}
