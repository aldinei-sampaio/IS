using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Choices;

public class ChoiceOptionHelpTextBuilder : IBuilder<IChoiceOptionPrototype>
{
    private readonly ITextSource textSource;

    public ChoiceOptionHelpTextBuilder(ITextSource textSource)
        => this.textSource = textSource;

    public void Build(IChoiceOptionPrototype prototype, INavigationContext context)
        => prototype.HelpText = textSource.ToString(context.Variables);
}