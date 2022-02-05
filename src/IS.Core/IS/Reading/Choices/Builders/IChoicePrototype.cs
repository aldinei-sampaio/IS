namespace IS.Reading.Choices.Builders;

public interface IChoicePrototype : IChoice
{
    new TimeSpan? TimeLimit { get; set; }
    new string? Default { get; set; }
    void Add(IChoiceOption option);
}
