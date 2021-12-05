using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public interface IChoiceOptionNode
{
    string Key { get; }
    string Text { get; }
    string? DisabledText { get; }
    string? ImageName { get; }
    string? HelpText { get; }
    ICondition? EnabledWhen { get; }
    ICondition? VisibleWhen { get; }
}
