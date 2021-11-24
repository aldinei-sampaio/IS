namespace IS.Reading.Events;

public class Choice
{
    public string Text { get; }
    public string Tip { get; }
    public string Value { get; }
    public Choice(string value, string text, string tip)
    {
        Value = value;
        Text = text;
        Tip = tip;
    }
}
