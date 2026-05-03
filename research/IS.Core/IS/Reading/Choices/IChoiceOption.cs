namespace IS.Reading.Choices;

public interface IChoiceOption
{
    string Key { get; }
    string Text { get; }
    bool IsEnabled { get; }
    string? ImageName { get; }
    string? Tip { get; }
}