using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.State;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundLeftNodeParser : IBackgroundLeftNodeParser
{
    private readonly IBackgroundImageTextParser backgroundImageTextParser;

    public BackgroundLeftNodeParser(IBackgroundImageTextParser backgroundImageTextParser)
        => this.backgroundImageTextParser = backgroundImageTextParser;

    public bool IsArgumentRequired => true;

    public string Name => "left";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var result = backgroundImageTextParser.Parse(reader.Argument);
        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return Task.CompletedTask;
        }

        var state = new BackgroundState(result.Value, BackgroundType.Image, BackgroundPosition.Left);
        var node = new BackgroundNode(state);
        parentParsingContext.AddNode(node);
        return Task.CompletedTask;
    }
}
