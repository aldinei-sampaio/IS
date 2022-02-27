using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.InputParsers;

public class InputConfNodeParser : IInputConfNodeParser
{
    public ITextSourceParser TextSourceParser { get; }

    public InputConfNodeParser(ITextSourceParser textSourceParser)
        => TextSourceParser = textSourceParser;

    public bool IsArgumentRequired => true;

    public string Name => "conf";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var parseResult = TextSourceParser.Parse(reader.Argument);
        if (!parseResult.IsOk)
        {
            parsingContext.LogError(reader, parseResult.ErrorMessage);
            return Task.CompletedTask;
        }

        var ctx = (InputParentParsingContext)parentParsingContext;
        ctx.InputBuilder.ConfirmationSource = parseResult.Value;
        return Task.CompletedTask;
    }
}