using IS.Reading.Conditions;
using IS.Reading.Navigation;

namespace IS.Reading.Choices;

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

    public void Build(T prototype, INavigationContext context)
    {
        var block = Condition.Evaluate(context.Variables) ? IfBlock : ElseBlock;
        foreach (var item in block)
            item.Build(prototype, context);
    }
}
