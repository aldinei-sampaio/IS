using IS.Reading.Choices;
using IS.Reading.Conditions;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceParentParsingContextBase<T> : IParentParsingContext
{
    public List<IBuilder<T>> Builders { get; } = new();

    public void AddSetter(Action<T> action)
        => Builders.Add(new BuilderPropertySetter<T>(action));

    public void AddDecision(
        ICondition condition,
        IEnumerable<IBuilder<T>> ifBlock,
        IEnumerable<IBuilder<T>> elseBlock
    )
        => Builders.Add(new BuilderDecision<T>(condition, ifBlock, elseBlock));
}