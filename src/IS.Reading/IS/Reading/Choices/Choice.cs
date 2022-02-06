namespace IS.Reading.Choices;

public class Choice : IChoice
{
    public string Key { get; }
    public TimeSpan? TimeLimit { get; }
    public string? Default { get; }
    public IEnumerable<IChoiceOption> Options { get; }
    public Choice(string key, IEnumerable<IChoiceOption> options, TimeSpan? timeLimit, string? @default)
    {
        Key = key;
        Options = options;
        TimeLimit = timeLimit;
        Default = @default;
    }
}
