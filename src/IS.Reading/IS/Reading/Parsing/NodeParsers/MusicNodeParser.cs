using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class MusicNodeParser : IMusicNodeParser
{
    private readonly INameTextParser nameTextParser;

    public MusicNodeParser(INameTextParser nameTextParser)
        => this.nameTextParser = nameTextParser;

    public bool IsArgumentRequired => true;

    public string Name => "music";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var parsed = nameTextParser.Parse(reader.Argument);
        if (!parsed.IsOk)
        {
            parsingContext.LogError(reader, parsed.ErrorMessage);
            return Task.CompletedTask;
        }

        if (parsingContext.SceneContext.HasMusic)
        {
            parsingContext.LogError(reader, "Mais de uma definição de música para a mesma cena.");
            return Task.CompletedTask;
        }

        parsingContext.SceneContext.HasMusic = true;

        var node = new MusicNode(parsed.Value.Length == 0 ? null : parsed.Value);
        parentParsingContext.AddNode(node);
        parsingContext.RegisterDismissNode(DismissNode);

        return Task.CompletedTask;
    }

    public INode DismissNode { get; } 
        = new MusicNode(null);
}
