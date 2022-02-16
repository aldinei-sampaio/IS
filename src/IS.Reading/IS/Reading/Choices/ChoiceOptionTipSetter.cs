using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Choices;

public class ChoiceOptionTipSetter : IBuilder<IChoiceOptionPrototype>
{
    public ITextSource TextSource { get; }

    public ChoiceOptionTipSetter(ITextSource textSource)
        => TextSource = textSource;

    public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
        => prototype.Tip = TextSource.Build(context.Variables);
}
