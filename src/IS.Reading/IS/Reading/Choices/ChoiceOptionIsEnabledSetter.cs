using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceOptionIsEnabledSetter : IBuilder<IChoiceOptionPrototype>
{
    public ChoiceOptionIsEnabledSetter(bool value)
        => Value = value;

    public bool Value { get; }

    public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
        => prototype.IsEnabled = Value;
}
