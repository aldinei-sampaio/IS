using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceTimeLimitSetter(TimeSpan? value) : IBuilder<IChoicePrototype>
{
    public TimeSpan? Value { get; } = value;

    public void Build(IChoicePrototype prototype, INavigationContext context)
        => prototype.TimeLimit = Value;
}
