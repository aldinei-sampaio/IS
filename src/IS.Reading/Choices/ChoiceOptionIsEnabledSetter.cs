using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceOptionIsEnabledSetter(bool value) : IBuilder<IChoiceOptionPrototype>
{
    public bool Value { get; } = value;

    public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
        => prototype.IsEnabled = Value;
}
