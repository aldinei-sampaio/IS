using IS.Reading.Conditions;
using IS.Reading.Navigation;
using IS.Reading.Nodes;

namespace IS.Reading.Parsing.NodeParsers;

public class DecisionBlock : IDecisionBlock
{
    public ICondition Condition { get; }
    public IBlock Block { get; }

    public DecisionBlock(ICondition condition, IBlock block)
    {
        Condition = condition;
        Block = block;
    }
}
