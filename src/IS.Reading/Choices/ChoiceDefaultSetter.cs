using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceDefaultSetter(string? value) : IBuilder<IChoicePrototype>
{
    public string? Value { get; } = value;

    public void Build(IChoicePrototype prototype, INavigationContext context)
        => prototype.Default = Value;
}
