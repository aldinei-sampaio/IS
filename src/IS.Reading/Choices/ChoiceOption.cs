namespace IS.Reading.Choices;

public class ChoiceOption(string key, string text, bool isEnabled, string? imageName, string? helpText) : IChoiceOption
{
    public string Key { get; } = key;
    public string Text { get; } = text;
    public bool IsEnabled { get; } = isEnabled;
    public string? ImageName { get; } = imageName;
    public string? Tip { get; } = helpText;
}
