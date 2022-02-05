using IS.Reading.Conditions;
using IS.Reading.Variables;

namespace IS.Reading.Choices.Builders;

public class BuilderDecision<T> : IBuilder<T>
{
    public BuilderDecision(
        ICondition condition,
        IEnumerable<IBuilder<T>> ifBlock,
        IEnumerable<IBuilder<T>> elseBlock
    )
    {
        Condition = condition;
        IfBlock = ifBlock;
        ElseBlock = elseBlock;
    }

    public ICondition Condition { get; set; }
    public IEnumerable<IBuilder<T>> IfBlock { get; }
    public IEnumerable<IBuilder<T>> ElseBlock { get; }

    public void Build(T prototype, IVariableDictionary variables)
    {
        var block = Condition.Evaluate(variables) ? IfBlock : ElseBlock;
        foreach (var item in block)
            item.Build(prototype, variables);
    }
}
