namespace IS.Reading.Navigation;

public interface IBlockFactory
{
    IBlock Create(IReadOnlyList<INode> nodes);
    IBlock Create(INode node1, INode node2);
}
