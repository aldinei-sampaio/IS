using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceOptionImageNameSetter : IBuilder<IChoiceOptionPrototype>
{
    public ChoiceOptionImageNameSetter(string? value)
        => Value = value;

    public string? Value { get; }

    public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
        => prototype.ImageName = Value;
}