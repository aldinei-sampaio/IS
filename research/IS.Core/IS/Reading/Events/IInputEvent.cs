namespace IS.Reading.Events;

public interface IInputEvent : IReadingEvent
{
    string Key { get; }
    string? Text { get; }
    int MaxLength { get; }
    string? Confirmation { get; }
    string? Title { get; }
    string? DefaultValue { get; }
}