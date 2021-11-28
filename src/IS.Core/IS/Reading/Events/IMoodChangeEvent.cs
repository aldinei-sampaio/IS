namespace IS.Reading.Events;

public interface IMoodChangeEvent : IReadingEvent
{
    string PersonName { get; }
    MoodType MoodType { get; }
}
