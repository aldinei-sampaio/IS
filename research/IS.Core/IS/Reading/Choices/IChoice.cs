namespace IS.Reading.Choices;

public interface IChoice
{
    string Key { get; }
    TimeSpan? TimeLimit { get; }
    string? Default { get; }
    IEnumerable<IChoiceOption> Options { get; }
}
