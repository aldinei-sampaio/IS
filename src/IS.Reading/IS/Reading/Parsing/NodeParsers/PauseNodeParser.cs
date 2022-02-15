using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class PauseNodeParser : IPauseNodeParser
{
    public const int MinTimeout = 1;
    public const int MaxTimeout = 5000;

    public IIntegerArgumentParser IntegerArgumentParser { get; }

    public PauseNodeParser(IIntegerArgumentParser integerArgumentParser)
        => IntegerArgumentParser = integerArgumentParser;

    public bool IsArgumentRequired => false;

    public string Name => "pause";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (!string.IsNullOrWhiteSpace(reader.Argument))
        {
            var result = IntegerArgumentParser.Parse(reader.Argument, MinTimeout, MaxTimeout);
            if (!result.IsOk)
            {
                parsingContext.LogError(reader, result.ErrorMessage);
                return Task.CompletedTask;
            }

            parsingContext.SceneContext.Reset();
            var timedPauseNode = new TimedPauseNode(TimeSpan.FromMilliseconds(result.Value));
            parentParsingContext.AddNode(timedPauseNode);
            return Task.CompletedTask;
        }

        parsingContext.SceneContext.Reset();
        parentParsingContext.AddNode(new PauseNode());
        return Task.CompletedTask;
    }
}
