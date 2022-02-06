using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Choices;

public class ChoiceOptionTextBuilder : IBuilder<IChoiceOptionPrototype>
{
    private readonly ITextSource textSource;

    public ChoiceOptionTextBuilder(ITextSource textSource)
        => this.textSource = textSource;

    public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
        => prototype.Text = textSource.ToString(context.Variables);
}
