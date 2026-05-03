namespace IS.Reading.Events
{
    public interface IBalloonTitleEvent : IReadingEvent
    {
        string Text { get; }
    }
}