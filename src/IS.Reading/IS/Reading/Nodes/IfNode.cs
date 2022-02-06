using IS.Reading.Conditions;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class IfNode : INode
{
    public IfNode(ICondition condition, IBlock ifBlock, IBlock elseBlock)
    {
        Condition = condition;
        IfBlock = ifBlock;
        ElseBlock = elseBlock;
    }

    public ICondition Condition { get; }
    public IBlock IfBlock { get; }
    public IBlock ElseBlock { get; }
    
    public IBlock? ChildBlock { get; private set; }

    public Task<object?> EnterAsync(INavigationContext context)
    {
        var evaluation = Condition.Evaluate(context.Variables);
        ChildBlock = evaluation ? IfBlock : ElseBlock;
        return Task.FromResult<object?>(evaluation);
    }

    public Task EnterAsync(INavigationContext context, object? state)
    {
        var evaluation = (state as bool?) ?? true;
        ChildBlock = evaluation ? IfBlock : ElseBlock;
        return Task.FromResult<object?>(evaluation);
    }
}