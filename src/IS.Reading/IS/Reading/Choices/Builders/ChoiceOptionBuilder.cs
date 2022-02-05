using IS.Reading.Variables;

namespace IS.Reading.Choices.Builders;

public class ChoiceOptionBuilder
{
    private class ChoiceOptionPrototype : IChoiceOptionPrototype
    {
        public ChoiceOptionPrototype(string key)
            => Key = key;

        public string Key { get; }
        public string Text { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string? ImageName { get; set; }
        public string? HelpText { get; set; }
    }

    public string Key { get; }

    public IEnumerable<IBuilder<IChoiceOptionPrototype>> Items { get; }

    public ChoiceOptionBuilder(string key, IEnumerable<IBuilder<IChoiceOptionPrototype>> items)
        => (Key, Items) = (key, items);

    public void Build(IChoicePrototype choicePrototype, IVariableDictionary variables)
    {
        var optionPrototype = new ChoiceOptionPrototype(Key);
        foreach(var item in Items)
            item.Build(optionPrototype, variables);
        if (!string.IsNullOrEmpty(optionPrototype.Text))
            choicePrototype.Add(optionPrototype);
    }
}