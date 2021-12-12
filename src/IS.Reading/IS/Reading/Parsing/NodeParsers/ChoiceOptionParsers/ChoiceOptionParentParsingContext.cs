using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public class ChoiceOptionParentParsingContext : IParentParsingContext
{
    public IChoiceOptionNodeSetter Option { get; set; } = new ChoiceOptionNode();
}
