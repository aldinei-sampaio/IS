using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Choices;

public class ChoiceOptionTipSetter(ITextSource textSource) : IBuilder<IChoiceOptionPrototype>
{
    public ITextSource TextSource { get; } = textSource;

    public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
        => prototype.Tip = TextSource.Build(context.Variables);
}
