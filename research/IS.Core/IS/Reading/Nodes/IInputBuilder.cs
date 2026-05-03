using IS.Reading.Events;
using IS.Reading.Variables;

namespace IS.Reading.Nodes
{
    public interface IInputBuilder
    {
        ITextSource? ConfirmationSource { get; set; }
        string Key { get; }
        int MaxLength { get; set; }
        ITextSource? TextSource { get; set; }
        ITextSource? TitleSource { get; set; }

        IInputEvent BuildEvent(IVariableDictionary variableDictionary);
    }
}