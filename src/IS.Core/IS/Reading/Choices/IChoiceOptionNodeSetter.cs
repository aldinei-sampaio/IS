using IS.Reading.Conditions;

namespace IS.Reading.Choices;

public interface IChoiceOptionNodeSetter : IChoiceOptionNode
{
    new string Key { get; set; }
    new string Text { get; set; }
    new string? DisabledText { get; set; }
    new string? ImageName { get; set; }
    new string? HelpText { get; set; }
    new ICondition? EnabledWhen { get; set; }
    new ICondition? VisibleWhen { get; set; }
}