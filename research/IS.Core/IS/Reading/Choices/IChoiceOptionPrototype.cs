namespace IS.Reading.Choices;

public interface IChoiceOptionPrototype : IChoiceOption
{
    new string Text { get; set; }
    new bool IsEnabled { get; set; }
    new string? ImageName { get; set; }
    new string? Tip { get; set; }
}
