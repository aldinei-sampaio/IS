using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class PauseNodeParser : IPauseNodeParser
{
    private readonly IIntegerArgumentParser integerTextParser;

    public PauseNodeParser(IIntegerArgumentParser integerTextParser)
        => this.integerTextParser = integerTextParser;

    public bool IsArgumentRequired => false;

    public string Name => "pause";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        parsingContext.SceneContext.Reset();

        if (!string.IsNullOrWhiteSpace(reader.Argument))
        {
            var result = integerTextParser.Parse(reader.Argument, 1, 5000);
            if (!result.IsOk)
            {
                parsingContext.LogError(reader, result.ErrorMessage);
                return Task.CompletedTask;
            }

            var timedPauseNode = new TimedPauseNode(TimeSpan.FromMilliseconds(result.Value));
            parentParsingContext.AddNode(timedPauseNode);
            return Task.CompletedTask;
        }

        parentParsingContext.AddNode(new PauseNode());
        return Task.CompletedTask;
    }
}
