using IS.Reading.Conditions;
using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public interface IParentParsingContext
{
    void AddNode(INode node) => throw new InvalidOperationException();
    string? ParsedText { get => null; set => throw new InvalidOperationException(); }
    ICondition? When { get => null; set => throw new InvalidOperationException(); }
    ICondition? While { get => null; set => throw new InvalidOperationException(); }
}
