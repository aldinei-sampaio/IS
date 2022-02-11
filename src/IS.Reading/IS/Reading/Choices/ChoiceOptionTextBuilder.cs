using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Choices;

public class ChoiceOptionTextBuilder : IBuilder<IChoiceOptionPrototype>
{
    public ITextSource TextSource { get; }

    public ChoiceOptionTextBuilder(ITextSource textSource)
        => this.TextSource = textSource;

    public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
        => prototype.Text = TextSource.Build(context.Variables);
}
