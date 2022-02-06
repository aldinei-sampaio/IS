using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public interface IParentParsingContext
{
    void AddNode(INode node) => throw new InvalidOperationException();
}
