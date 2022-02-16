using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceTimeLimitSetter : IBuilder<IChoicePrototype>
{
    public ChoiceTimeLimitSetter(TimeSpan? value)
        => Value = value;

    public TimeSpan? Value { get; }

    public void Build(IChoicePrototype prototype, INavigationContext context)
        => prototype.TimeLimit = Value;
}
