using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Choices;

public class ChoiceOptionTipBuilder : IBuilder<IChoiceOptionPrototype>
{
    private readonly ITextSource textSource;

    public ChoiceOptionTipBuilder(ITextSource textSource)
        => this.textSource = textSource;

    public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
        => prototype.Tip = textSource.ToString(context.Variables);
}