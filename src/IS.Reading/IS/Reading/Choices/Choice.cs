namespace IS.Reading.Choices;

public class Choice : IChoice
{
    public TimeSpan? TimeLimit { get; }
    public string? Default { get; }
    public IEnumerable<IChoiceOption> Options { get; }
    public Choice(IEnumerable<IChoiceOption> options, TimeSpan? timeLimit, string? @default)
    {
        Options = options;
        TimeLimit = timeLimit;
        Default = @default;
    }
}
