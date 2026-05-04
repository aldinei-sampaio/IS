using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceBuilder(string key, IEnumerable<IBuilder<IChoicePrototype>> items) : IChoiceBuilder
{
    private class ChoicePrototype : IChoicePrototype
    {
        private readonly List<IChoiceOption> options = new();

        public ChoicePrototype(string key)
            => Key = key;

        public string Key { get; }
        public TimeSpan? TimeLimit { get; set; }
        public string? Default { get; set; }
        public bool RandomOrder { get; set; }
        public IEnumerable<IChoiceOption> Options => options;
        public void Add(IChoiceOption option) => options.Add(option);
    }

    public string Key { get; } = key;

    public IEnumerable<IBuilder<IChoicePrototype>> Items { get; } = items;

    public IChoice? Build(INavigationContext context)
    {
        var prototype = new ChoicePrototype(Key);

        foreach (var item in Items)
            item.Build(prototype, context);

        var options = prototype.Options;

        if (!options.Any(i => i.IsEnabled))
            return null;

        if (prototype.RandomOrder)
            options = context.Randomizer.Shuffle(options);

        return new Choice(prototype.Key, options, prototype.TimeLimit, prototype.Default);
    }
}
