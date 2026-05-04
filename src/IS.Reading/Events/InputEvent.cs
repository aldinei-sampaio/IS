namespace IS.Reading.Events;

public class InputEvent(
    string key,
    string? title,
    string? text,
    int maxLength,
    string? confirmation,
    string? defaultValue
) : IInputEvent
{
    public string Key { get; } = key;
    public string? Title { get; } = title;
    public string? Text { get; } = text;
    public int MaxLength { get; } = maxLength;
    public string? Confirmation { get; } = confirmation;
    public string? DefaultValue { get; } = defaultValue;

    public override string ToString()
        => $"input: {Key}";
}
