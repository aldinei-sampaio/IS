namespace IS.Reading.Events;

public class InputEvent : IInputEvent
{
    public InputEvent(
        string key, 
        string? title, 
        string? text, 
        int maxLength, 
        string? confirmation,
        string? defaultValue
    )
    {
        Key = key;
        Title = title;
        Text = text;
        MaxLength = maxLength;
        Confirmation = confirmation;
        DefaultValue = defaultValue;
    }
    public string Key { get; }
    public string? Title { get; }
    public string? Text { get; }
    public int MaxLength { get; }
    public string? Confirmation { get; }
    public string? DefaultValue { get; }

    public override string ToString()
        => $"input: {Key}";
}