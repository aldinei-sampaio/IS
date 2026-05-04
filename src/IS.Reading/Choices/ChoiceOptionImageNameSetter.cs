using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceOptionImageNameSetter(string? value) : IBuilder<IChoiceOptionPrototype>
{
    public string? Value { get; } = value;

    public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
        => prototype.ImageName = Value;
}
