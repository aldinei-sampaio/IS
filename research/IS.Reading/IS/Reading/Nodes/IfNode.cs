using IS.Reading.Conditions;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class IfNode : INode
{
    public IfNode(IReadOnlyList<IDecisionBlock> decisionBlocks, IBlock elseBlock)
    {
        DecisionBlocks = decisionBlocks;
        ElseBlock = elseBlock;
    }

    public IReadOnlyList<IDecisionBlock> DecisionBlocks { get; }
    public IBlock ElseBlock { get; }    
    public IBlock? ChildBlock { get; private set; }

    public Task<object?> EnterAsync(INavigationContext context)
    {
        var variableDictionary = context.Variables;
        int? blockIndex = null;
        for(var n = 0; n < DecisionBlocks.Count; n++)
        {
            if (DecisionBlocks[n].Condition.Evaluate(variableDictionary))
            {
                blockIndex = n;
                break;
            }
        }
        ChildBlock = blockIndex.HasValue ? DecisionBlocks[blockIndex.Value].Block : ElseBlock;
        return Task.FromResult<object?>(blockIndex);
    }

    public Task EnterAsync(INavigationContext context, object? state)
    {
        var blockIndex = state as int?;
        ChildBlock = blockIndex.HasValue ? DecisionBlocks[blockIndex.Value].Block : ElseBlock;
        return Task.CompletedTask;
    }
}