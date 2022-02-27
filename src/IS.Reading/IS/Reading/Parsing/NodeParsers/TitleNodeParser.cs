using IS.Reading.Nodes;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class TitleNodeParser : ITitleNodeParser
{
    public TitleNodeParser(ITextSourceParser textSourceParser)
        => TextSourceParser = textSourceParser;

    public string Name => "title";
    public bool IsArgumentRequired => true;
    public ITextSourceParser TextSourceParser { get; }

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var result = TextSourceParser.Parse(reader.Argument);

        if (result.IsOk)
            parentParsingContext.AddNode(new BalloonTitleNode(result.Value));
        else
            parsingContext.LogError(reader, result.ErrorMessage);

        return Task.CompletedTask;
    }
}
