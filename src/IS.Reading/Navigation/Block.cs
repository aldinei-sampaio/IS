using IS.Reading.Conditions;

namespace IS.Reading.Navigation;

public class Block(int id, IReadOnlyList<INode> nodes, ICondition? @while) : IBlock
{
    public int Id { get; } = id;

    public IReadOnlyList<INode> Nodes { get; } = nodes;

    public ICondition? While { get; } = @while;
}
