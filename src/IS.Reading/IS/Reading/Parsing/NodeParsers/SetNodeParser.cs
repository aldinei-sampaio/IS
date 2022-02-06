using IS.Reading.Nodes;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class SetNodeParser : ISetNodeParser
{
    private readonly IVarSetParser varSetParser;

    public SetNodeParser(IVarSetParser varSetParser)
        => this.varSetParser = varSetParser;

    public bool IsArgumentRequired => true;

    public string Name => "set";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (string.IsNullOrWhiteSpace(reader.Argument))
        {
            parsingContext.LogError(reader, "Era esperada uma expressão de atribuição.");
            return Task.CompletedTask;
        }

        var varSet = varSetParser.Parse(reader.Argument);

        if (varSet.IsOk)
            parentParsingContext.AddNode(new VarSetNode(varSet.Value));
        else
            parsingContext.LogError(reader, varSet.ErrorMessage);

        return Task.CompletedTask;
    }
}
