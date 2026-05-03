using IS.Reading.Conditions;

namespace IS.Reading.Navigation;

public interface IBlockFactory
{
    IBlock Create(IReadOnlyList<INode> nodes, ICondition? @while);
    IBlock Create(IReadOnlyList<INode> nodes);
    IBlock Create(INode node1, INode node2);
}
