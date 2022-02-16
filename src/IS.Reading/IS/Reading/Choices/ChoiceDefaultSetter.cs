using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceDefaultSetter : IBuilder<IChoicePrototype>
{
    public ChoiceDefaultSetter(string? value)
        => Value = value;

    public string? Value { get; }

    public void Build(IChoicePrototype prototype, INavigationContext context)
        => prototype.Default = Value;
}
