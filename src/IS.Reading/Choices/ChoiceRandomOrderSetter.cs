using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceRandomOrderSetter(bool value) : IBuilder<IChoicePrototype>
{
    public bool Value { get; } = value;

    public void Build(IChoicePrototype prototype, INavigationContext context)
        => prototype.RandomOrder = Value;
}
