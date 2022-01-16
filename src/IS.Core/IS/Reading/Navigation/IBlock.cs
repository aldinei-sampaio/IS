namespace IS.Reading.Navigation;

public interface IBlock
{
    int Id { get; }
    IReadOnlyList<INode> Nodes { get; }
}
