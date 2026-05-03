using IS.Reading.Nodes;

namespace IS.Reading.Parsing.NodeParsers.InputParsers;

public class InputParentParsingContext : IParentParsingContext
{
    public InputParentParsingContext(string key)
        => InputBuilder = new(key);

    public InputBuilder InputBuilder { get; }
}
