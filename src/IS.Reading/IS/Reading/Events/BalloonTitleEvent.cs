namespace IS.Reading.Events;

public class BalloonTitleEvent : IBalloonTitleEvent
{
    public BalloonTitleEvent(string text)
        => Text = text;

    public string Text { get; }

    public override string ToString()
        => $"title: {Text}";
}