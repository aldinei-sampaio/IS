using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public interface IChoiceOptionParentParsingContext : IParentParsingContext
{
    public IChoiceOptionNodeSetter Option { get; set; }
}
