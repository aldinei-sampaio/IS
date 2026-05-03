using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.State;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundColorNodeParser : IBackgroundColorNodeParser
{
    public IColorArgumentParser ColorArgumentParser { get; }

    public BackgroundColorNodeParser(IColorArgumentParser colorArgumentParser)
        => ColorArgumentParser = colorArgumentParser;

    public bool IsArgumentRequired => true;

    public string Name => "color";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var parsed = ColorArgumentParser.Parse(reader.Argument);
        if (!parsed.IsOk)
        {
            parsingContext.LogError(reader, parsed.ErrorMessage);
            return Task.CompletedTask;
        }

        var state = new BackgroundState(parsed.Value, BackgroundType.Color, BackgroundPosition.Undefined);
        parentParsingContext.AddNode(new BackgroundNode(state));

        return Task.CompletedTask;
    }
}
