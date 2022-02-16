using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.State;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundRightNodeParser : IBackgroundRightNodeParser
{
    public IImageArgumentParser ImageArgumentParser { get; }

    public BackgroundRightNodeParser(IImageArgumentParser imageArgumentParser)
        => ImageArgumentParser = imageArgumentParser;

    public bool IsArgumentRequired => true;

    public string Name => "right";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var result = ImageArgumentParser.Parse(reader.Argument);
        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return Task.CompletedTask;
        }

        var state = new BackgroundState(result.Value, BackgroundType.Image, BackgroundPosition.Right);
        var node = new BackgroundNode(state);
        parentParsingContext.AddNode(node);
        return Task.CompletedTask;
    }
}
