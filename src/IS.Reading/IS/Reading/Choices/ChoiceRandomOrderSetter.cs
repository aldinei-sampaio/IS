using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceRandomOrderSetter : IBuilder<IChoicePrototype>
{
    public ChoiceRandomOrderSetter(bool value)
        => Value = value;

    public bool Value { get; }

    public void Build(IChoicePrototype prototype, INavigationContext context)
        => prototype.RandomOrder = Value;
}
