namespace IS.Reading.Choices;

public interface IChoice
{
    TimeSpan? TimeLimit { get; }
    string? Default { get; }
    IEnumerable<IChoiceOption> Options { get; }
}
