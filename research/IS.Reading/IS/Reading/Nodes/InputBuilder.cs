using IS.Reading.Events;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class InputBuilder : IInputBuilder
{
    public const int MaxLenghtDefaultValue = 16;

    public string Key { get; }
    public ITextSource? TitleSource { get; set; }
    public ITextSource? TextSource { get; set; }
    public int MaxLength { get; set; } = MaxLenghtDefaultValue;
    public ITextSource? ConfirmationSource { get; set; }

    public InputBuilder(string key)
        => Key = key;

    public IInputEvent BuildEvent(IVariableDictionary variableDictionary)
    {
        var title = TitleSource?.Build(variableDictionary);
        var text = TextSource?.Build(variableDictionary);
        var confirmation = ConfirmationSource?.Build(variableDictionary);
        var defaultValue = variableDictionary[Key] as string;

        return new InputEvent(Key, title, text, MaxLength, confirmation, defaultValue);
    }
}
