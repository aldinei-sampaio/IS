using IS.Reading.Navigation;

namespace IS.Reading.Parsing.NodeParsers;

public class BlockParentParsingContext : IParentParsingContext
{
    public Block Block { get; } = new();
    public string? ParsedText { get; set; }
    public ICondition? When { get; set; }
    public ICondition? While { get; set; }

    public void AddNode(INode node)
        => Block.ForwardQueue.Enqueue(node);
}