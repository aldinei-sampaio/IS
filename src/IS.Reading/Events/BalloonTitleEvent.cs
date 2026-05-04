namespace IS.Reading.Events;

public class BalloonTitleEvent(string text) : IBalloonTitleEvent
{
    public string Text { get; } = text;

    public override string ToString()
        => string.IsNullOrEmpty(Text) ? "title unset" : $"title: {Text}";
}
