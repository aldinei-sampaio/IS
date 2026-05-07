using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.State;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundColorNodeParser : IBackgroundColorNodeParser
{
    public IBackgroundColorArgumentParser BackgroundColorArgumentParser { get; }

    public BackgroundColorNodeParser(IBackgroundColorArgumentParser backgroundColorArgumentParser)
        => BackgroundColorArgumentParser = backgroundColorArgumentParser;

    public bool IsArgumentRequired => true;

    public string Name => "color";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var result = BackgroundColorArgumentParser.Parse(reader.Argument);
        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return Task.CompletedTask;
        }

        var arg = result.Value;
        var state = new BackgroundState(arg.ColorValue, BackgroundType.Color, BackgroundPosition.Undefined);
        parentParsingContext.AddNode(new BackgroundNode(state, arg.Animation, arg.FlashColor));

        return Task.CompletedTask;
    }
}
