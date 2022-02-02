using IS.Reading.Navigation;

namespace IS.Reading.Parsing.NodeParsers;

public class ParentParsingContext : IParentParsingContext
{
    public List<INode> Nodes { get; } = new();

    public void AddNode(INode node) => Nodes.Add(node);
}