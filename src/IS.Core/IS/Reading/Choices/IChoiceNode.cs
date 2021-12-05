namespace IS.Reading.Choices;

public interface IChoiceNode
{
    TimeSpan? TimeLimit { get; }
    string? Default { get; }
    IEnumerable<IChoiceOptionNode> Options { get; }
}
