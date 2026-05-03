using IS.Reading.Nodes;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class SetNodeParser : ISetNodeParser
{
    public IVarSetParser VarSetParser { get; }

    public SetNodeParser(IVarSetParser varSetParser)
        => VarSetParser = varSetParser;

    public bool IsArgumentRequired => true;

    public string Name => "set";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var varSet = VarSetParser.Parse(reader.Argument);

        if (varSet.IsOk)
            parentParsingContext.AddNode(new VarSetNode(varSet.Value));
        else
            parsingContext.LogError(reader, varSet.ErrorMessage);

        return Task.CompletedTask;
    }
}
