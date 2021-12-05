namespace IS.Reading.Choices;

public class ChoiceOption : IChoiceOption
{
    public string Key { get; }
    public string Text { get; }
    public bool IsEnabled { get; }
    public string? ImageName { get; }
    public string? HelpText { get; }
    
    public ChoiceOption(string key, string text, bool isEnabled, string? imageName, string? helpText)
    {
        Key = key;
        Text = text;
        IsEnabled = isEnabled;
        ImageName = imageName;
        HelpText = helpText;
    }
}