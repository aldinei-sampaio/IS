namespace IS.Reading.Events;

public class Prompt
{
    public IReadOnlyList<Choice> Choices { get; }
    public TimeSpan? TimeLimit { get; }
    public string? DefaultChoice { get; }
    public bool RandomOrder { get; }

    public Prompt(IReadOnlyList<Choice> choices, TimeSpan? timeLimit, string? defaultChoice, bool randomOrder)
    {
        Choices = choices;
        TimeLimit = timeLimit;
        DefaultChoice = defaultChoice;
        RandomOrder = randomOrder;
    }
}
