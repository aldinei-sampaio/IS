using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceOptionNode : IChoiceOptionNodeSetter
{
    public string Key { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? DisabledText { get; set; }
    public string? ImageName { get; set; }
    public string? HelpText { get; set; }
    public ICondition? EnabledWhen { get; set; }
    public ICondition? VisibleWhen { get; set; }
}
