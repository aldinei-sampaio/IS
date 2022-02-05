using IS.Reading.Variables;

namespace IS.Reading.Choices.Builders;

public class ChoiceBuilder : IChoiceBuilder
{
    private class ChoicePrototype : IChoicePrototype
    {
        private readonly List<IChoiceOption> options = new();

        public TimeSpan? TimeLimit { get; set; }
        public string? Default { get; set; }
        public IEnumerable<IChoiceOption> Options => options;
        public void Add(IChoiceOption option) => options.Add(option);
    }

    public IEnumerable<IBuilder<IChoicePrototype>> Items { get; }

    public ChoiceBuilder(IEnumerable<IBuilder<IChoicePrototype>> items)
        => this.Items = items;

    public IChoice? Build(IVariableDictionary variables)
    {
        var prototype = new ChoicePrototype();
        foreach (var item in Items)
            item.Build(prototype, variables);

        if (!prototype.Options.Any())
            return null;

        return new Choice(prototype.Options, prototype.TimeLimit, prototype.Default);
    }
}
