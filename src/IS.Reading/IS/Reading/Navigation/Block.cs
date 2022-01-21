using IS.Reading.Conditions;

namespace IS.Reading.Navigation;

public class Block : IBlock
{
    public int Id { get; }

    public IReadOnlyList<INode> Nodes { get; }

    public ICondition? While { get; }

    public Block(int id, IReadOnlyList<INode> nodes, ICondition? @while)
        => (Id, Nodes, While) = (id, nodes, @while);
}
