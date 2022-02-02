using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.ArgumentParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class MusicNodeParser : IMusicNodeParser
{
    private readonly INameTextParser nameTextParser;

    public MusicNodeParser(INameTextParser nameTextParser)
        => this.nameTextParser = nameTextParser;

    public string Name => "music";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var parsed = nameTextParser.Parse(reader, parsingContext, reader.Argument);
        if (parsed is null)
            return Task.CompletedTask;

        if (parsingContext.SceneContext.HasMusic)
        {
            parsingContext.LogError(reader, "Mais de uma definição de música para a mesma cena.");
            return Task.CompletedTask;
        }

        parsingContext.SceneContext.HasMusic = true;

        var node = new MusicNode(parsed.Length == 0 ? null : parsed);
        parentParsingContext.AddNode(node);
        parsingContext.RegisterDismissNode(DismissNode);

        return Task.CompletedTask;
    }

    public INode DismissNode { get; } 
        = new MusicNode(null);
}
