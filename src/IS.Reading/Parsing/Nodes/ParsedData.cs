using IS.Reading.Navigation;

namespace IS.Reading.Parsing.Nodes;

public class ParsedData
{
    public IBlock? Block { get; set; }
    public ICondition? When { get; set; }
    public ICondition? While { get; set; }
    public string? Text { get; set; }
}
