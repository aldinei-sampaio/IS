namespace IS.Reading.Choices;

public class ChoiceNode : IChoiceNode
{
    public TimeSpan? TimeLimit { get; set; }
    public string? Default { get; set; }
    public List<IChoiceOptionNode> Options { get; } = new();

    IEnumerable<IChoiceOptionNode> IChoiceNode.Options => Options;
}
