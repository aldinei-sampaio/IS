using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class BuilderParentParsingContext<T> : IParentParsingContext
{
    public List<IBuilder<T>> Builders { get; } = new();
}