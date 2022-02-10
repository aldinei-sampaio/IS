using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class ChoiceOptionBuilder : IBuilder<IChoicePrototype>
{
    private class ChoiceOptionPrototype : IChoiceOptionPrototype
    {
        public ChoiceOptionPrototype(string key)
            => Key = key;

        public string Key { get; }
        public string Text { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string? ImageName { get; set; }
        public string? Tip { get; set; }
    }

    public string Key { get; }

    public IEnumerable<IBuilder<IChoiceOptionPrototype>> Items { get; }

    public ChoiceOptionBuilder(string key, IEnumerable<IBuilder<IChoiceOptionPrototype>> items)
        => (Key, Items) = (key, items);

    public void Build(IChoicePrototype choicePrototype, INavigationContext context)
    {
        var optionPrototype = new ChoiceOptionPrototype(Key);
        foreach(var item in Items)
            item.Build(optionPrototype, context);
        if (string.IsNullOrEmpty(optionPrototype.Text))
            return;

        choicePrototype.Add(new ChoiceOption(
            optionPrototype.Key, 
            optionPrototype.Text,
            optionPrototype.IsEnabled,
            optionPrototype.ImageName,
            optionPrototype.Tip
        ));
    }
}