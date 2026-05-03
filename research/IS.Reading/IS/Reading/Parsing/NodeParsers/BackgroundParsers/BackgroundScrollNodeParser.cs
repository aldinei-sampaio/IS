using IS.Reading.Nodes;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundScrollNodeParser : IBackgroundScrollNodeParser
{
    public bool IsArgumentRequired => false;

    public string Name => "scroll";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (!string.IsNullOrEmpty(reader.Argument))
            parsingContext.LogError(reader, "O comando 'scroll' não suporta argumento.");
        else
            parentParsingContext.AddNode(new ScrollNode());

        return Task.CompletedTask;
    }
}
