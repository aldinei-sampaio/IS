using IS.Reading.Conditions;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public interface IDecisionBlock
{
    ICondition Condition { get; }
    IBlock Block { get; }
}
