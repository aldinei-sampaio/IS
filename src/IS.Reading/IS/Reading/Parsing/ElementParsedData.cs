using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public class ElementParsedData : IElementParsedData
{
    public IBlock? Block { get; set; }
    public ICondition? When { get; set; }
    public ICondition? While { get; set; }
    public string? Text { get; set; }
}
