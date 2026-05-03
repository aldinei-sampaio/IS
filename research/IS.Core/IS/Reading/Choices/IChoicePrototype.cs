namespace IS.Reading.Choices;

public interface IChoicePrototype : IChoice
{
    new TimeSpan? TimeLimit { get; set; }
    new string? Default { get; set; }
    bool RandomOrder { get; set; }
    void Add(IChoiceOption option);
}
