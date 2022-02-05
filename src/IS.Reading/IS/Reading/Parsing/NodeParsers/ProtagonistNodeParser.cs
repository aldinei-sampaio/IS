using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class ProtagonistNodeParser : IProtagonistNodeParser
{
    private readonly INameTextParser nameTextParser;

    public ProtagonistNodeParser(INameTextParser nameTextParser)
        => this.nameTextParser = nameTextParser;

    public string Name => "protagonist";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (string.IsNullOrWhiteSpace(reader.Argument))
        {
            parsingContext.LogError(reader, "O nome do protagonista não foi informado.");
            return Task.CompletedTask;
        }

        var result = nameTextParser.Parse(reader, parsingContext, reader.Argument);
        if (result is null)
            return Task.CompletedTask;

        var node = new ProtagonistNode(result.Length == 0 ? null : result);
        parentParsingContext.AddNode(node);
        parsingContext.RegisterDismissNode(DismissNode);

        return Task.CompletedTask;
    }

    public INode DismissNode { get; } 
        = new ProtagonistNode(null);
}
