using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Choices;

public class ChoiceOptionTipBuilder : IBuilder<IChoiceOptionPrototype>
{
    public ITextSource TextSource { get; }

    public ChoiceOptionTipBuilder(ITextSource textSource)
        => this.TextSource = textSource;

    public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
        => prototype.Tip = TextSource.Build(context.Variables);
}