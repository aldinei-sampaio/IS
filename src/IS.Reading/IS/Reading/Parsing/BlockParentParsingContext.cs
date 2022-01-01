using IS.Reading.Conditions;
using IS.Reading.Navigation;

namespace IS.Reading.Parsing.NodeParsers;

public class BlockParentParsingContext : IParentParsingContext
{
    public List<INode> Nodes { get; } = new();
    public string? ParsedText { get; set; }
    public ICondition? When { get; set; }
    public ICondition? While { get; set; }

    public void AddNode(INode node)
        => Nodes.Add(node);
}