namespace IS.Reading.Choices;

public class Choice(string key, IEnumerable<IChoiceOption> options, TimeSpan? timeLimit, string? @default) : IChoice
{
    public string Key { get; } = key;
    public TimeSpan? TimeLimit { get; } = timeLimit;
    public string? Default { get; } = @default;
    public IEnumerable<IChoiceOption> Options { get; } = options;
}
