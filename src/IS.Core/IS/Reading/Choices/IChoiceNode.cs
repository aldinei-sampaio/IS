namespace IS.Reading.Choices;

public interface IChoiceNode
{
    TimeSpan? TimeLimit { get; }
    string? Default { get; }
    bool RandomOrder { get; }
    IEnumerable<IChoiceOptionNode> Options { get; }
}
