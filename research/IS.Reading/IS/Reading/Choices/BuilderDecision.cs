using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class BuilderDecision<T> : IBuilder<T>
{
    public BuilderDecision(
        IEnumerable<IBuilderDecisionItem<T>> items,
        IEnumerable<IBuilder<T>> elseBlock
    )
    {
        Items = items;
        ElseBlock = elseBlock;
    }

    public IEnumerable<IBuilderDecisionItem<T>> Items { get; }

    public IEnumerable<IBuilder<T>> ElseBlock { get; }

    public void Build(T prototype, INavigationContext context)
    {
        IEnumerable<IBuilder<T>>? block = null;
        foreach(var item in Items)
        {
            if (item.Condition.Evaluate(context.Variables))
            {
                block = item.Block;
                break;
            }
        }

        if (block is null)
            block = ElseBlock;

        foreach (var item in block)
            item.Build(prototype, context);
    }
}
